using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UI_TEST
{
    public class LabelSizingGroup : DependencyObject
    {
        private readonly HashSet<LabelsPanel> m_Panels = new HashSet<LabelsPanel>(ByRefComparer<LabelsPanel>.Default);

        public void Register(LabelsPanel panel)
        {
            Unregister(panel);
            m_Panels.Add(panel);
            panel.Unloaded += HandlePanelUnloaded;
        }

        public void Unregister(LabelsPanel panel)
        {
            m_Panels.Remove(panel);
            panel.Unloaded -= HandlePanelUnloaded;
        }

        public double GetMaxLabelWidth()
        {
            return m_Panels.Select(panel => panel.ActualLabelWidth).StartWith(0).Max();
        }

        private void HandlePanelUnloaded(object sender, RoutedEventArgs e)
        {
            m_Panels.Remove(sender as LabelsPanel);
        }
    }

    /// <summary>
    /// A panel with two columns, one for a label, one for a child.
    /// </summary>
    public class LabelsPanel : Panel
    {
        /// <summary>
        /// True means the child just adopts the width from the others
        /// </summary>
        public static readonly DependencyProperty AdoptsWidthProperty = DependencyProperty.RegisterAttached(
            "AdoptsWidth", typeof(bool), typeof(LabelsPanel), new PropertyMetadata(default(bool)));

        public static void SetAdoptsWidth(DependencyObject element, bool value)
        {
            element.SetValue(AdoptsWidthProperty, value);
        }

        public static bool GetAdoptsWidth(DependencyObject element)
        {
            return (bool)element.GetValue(AdoptsWidthProperty);
        }

        private const FrameworkPropertyMetadataOptions AffectsLayout =
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure;

        private const FrameworkPropertyMetadataOptions AffectsArrange = FrameworkPropertyMetadataOptions.AffectsArrange;

        public static readonly DependencyProperty ActualLabelWidthProperty =
            DependencyProperty.Register(nameof(ActualLabelWidth), typeof(double), typeof(LabelsPanel),
                new FrameworkPropertyMetadata(0D, AffectsLayout));

        public double ActualLabelWidth
        {
            get { return (double)GetValue(ActualLabelWidthProperty); }
            set { SetValue(ActualLabelWidthProperty, value); }
        }

        public static readonly DependencyProperty VerticalSpacingProperty =
            DependencyProperty.Register(nameof(VerticalSpacing), typeof(double), typeof(LabelsPanel),
                new FrameworkPropertyMetadata(0D, AffectsLayout));

        public double VerticalSpacing
        {
            get { return (double)GetValue(VerticalSpacingProperty); }
            set { SetValue(VerticalSpacingProperty, value); }
        }

        public static readonly DependencyProperty HorizontalSpacingProperty =
            DependencyProperty.Register(nameof(HorizontalSpacing), typeof(double), typeof(LabelsPanel),
                new FrameworkPropertyMetadata(0D, AffectsLayout));

        public double HorizontalSpacing
        {
            get { return (double)GetValue(HorizontalSpacingProperty); }
            set { SetValue(HorizontalSpacingProperty, value); }
        }

        public static readonly DependencyProperty MinLabelWidthProperty =
            DependencyProperty.Register("MinLabelWidth", typeof(double), typeof(LabelsPanel),
                new FrameworkPropertyMetadata(0D, AffectsLayout));

        public double MinLabelWidth
        {
            get { return (double)GetValue(MinLabelWidthProperty); }
            set { SetValue(MinLabelWidthProperty, value); }
        }

        public static readonly DependencyProperty SizingGroupProperty = DependencyProperty.RegisterAttached(
            "SizingGroup", typeof(LabelSizingGroup), typeof(LabelsPanel), new FrameworkPropertyMetadata(default(LabelSizingGroup), FrameworkPropertyMetadataOptions.Inherits | AffectsArrange, OnSizingGroupChanged));

        private static void OnSizingGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // This will also be called for each descendant 
            if (d is LabelsPanel panel)
            {
                var oldGroup = GetSizingGroup(panel);
                oldGroup?.Unregister(panel);
                var newGroup = e.NewValue as LabelSizingGroup;
                newGroup?.Register(panel);
            }
        }

        public static void SetSizingGroup(DependencyObject element, LabelSizingGroup value)
        {
            element.SetValue(SizingGroupProperty, value);
        }

        public static LabelSizingGroup GetSizingGroup(DependencyObject element)
        {
            return (LabelSizingGroup)element.GetValue(SizingGroupProperty);
        }

        static LabelsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelsPanel),
                new FrameworkPropertyMetadata(typeof(LabelsPanel)));
        }

        [DebuggerNonUserCode]
        protected override Size MeasureOverride(Size availableSize)
        {
            // Step 1: measure labels.
            var children = InternalChildren.OfType<UIElement>().ToArray();

            double labelWidth = MinLabelWidth;

            for (int i = 0; i < children.Length; i += 2)
            {
                var child = children[i];
                try
                {
                    child.Measure(availableSize);
                    if (!GetAdoptsWidth(child))
                        labelWidth = Math.Max(child.DesiredSize.Width, labelWidth);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WARNING: LabelPanel.Measure: child #{i} raised {ex}");
                }
            }

            ActualLabelWidth = labelWidth;

            // Step 2: measure controls in remaining available space.
            availableSize = new Size(availableSize.Width - labelWidth - HorizontalSpacing, availableSize.Height);

            double controlWidth = 0;

            for (int i = 1; i < children.Length; i += 2)
            {
                try
                {
                    var child = children[i];
                    child.Measure(availableSize);
                    if (!GetAdoptsWidth(child))
                        controlWidth = Math.Max(child.DesiredSize.Width, controlWidth);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WARNING: LabelPanel.Measure: child #{i} raised {ex}");
                }
            }

            // Step 3: measure total size, by stacking each row
            Size totalSize = new Size(labelWidth + HorizontalSpacing + controlWidth, 0);
            for (int index = 0; index < children.Length / 2; ++index)
            {
                var label = children[index * 2 + 0];
                var child = children[index * 2 + 1];
                var rowHeight = Math.Max(label.DesiredSize.Height, child.DesiredSize.Height);
                totalSize.Height += rowHeight + (index > 0 ? VerticalSpacing : 0);
            }

            return totalSize;
        }

        [DebuggerNonUserCode]
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var children = InternalChildren.OfType<UIElement>().ToArray();

            var rowPos = new Point();

            var finalRect = Rect.Empty;

            //var labelAlignment = LabelAlignment;
            //var childAlignment = ChildAlignment;

            var group = GetSizingGroup(this);
            var actualLabelWidth = group?.GetMaxLabelWidth() ?? ActualLabelWidth;
            var horizontalSpacing = HorizontalSpacing;
            var verticalSpacing = VerticalSpacing;

            for (int index = 0; index < children.Length / 2; ++index)
            {
                var label = children[index * 2 + 0];
                var child = children[index * 2 + 1];

                var rowHeight = Math.Max(label.DesiredSize.Height, child.DesiredSize.Height);

                Point labelPos = rowPos;
                Point childPos = rowPos + new Vector(actualLabelWidth + horizontalSpacing, 0);

                var labelSize = new Size(
                    actualLabelWidth,
                    rowHeight);

                var childSize = new Size(
                    arrangeBounds.Width - actualLabelWidth - horizontalSpacing,
                    rowHeight);

                try
                {
                    var labelRect = new Rect(labelPos, labelSize);
                    var childRect = new Rect(childPos, childSize);
                    label.Arrange(labelRect);
                    child.Arrange(childRect);

                    finalRect.Union(labelRect);
                    finalRect.Union(childRect);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WARNING: LabelPanel.Arrange: child {index} raised {ex}");
                }

                rowPos.Y += rowHeight + verticalSpacing;
            }

            return finalRect.Size.IsEmpty ? arrangeBounds : finalRect.Size;
        }
    }
}