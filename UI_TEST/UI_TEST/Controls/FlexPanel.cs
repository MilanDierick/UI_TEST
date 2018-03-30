using System;
using System.Windows;
using System.Windows.Controls;

namespace UI_TEST
{
    /// <summary>
    /// A stacking panel that adds spacing between to the child controls, and allows stretching each child by some weight factor.
    /// Similar but less powerful than the HTML5 flexbox.
    /// </summary>
    /// <remarks>
    /// This panel does not support scrolling, use a scroll-viewer for that.
    /// </remarks>
    public class FlexPanel : Panel
    {
        static FlexPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlexPanel),
                new FrameworkPropertyMetadata(typeof(FlexPanel)));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(FlexPanel),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalAlignmentProperty.OverrideMetadata(typeof(FlexPanel),
                new FrameworkPropertyMetadata(VerticalAlignment.Top));
        }

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
            "Spacing", typeof(double), typeof(FlexPanel),
            new FrameworkPropertyMetadata(10D,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(FlexPanel),
            new FrameworkPropertyMetadata(default(Orientation), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        /// <summary>
        /// Zero: DesiredSize without any stretching
        /// Negative: DesiredSize plus stretch the remaining space, proportionally
        /// Positive: Ignore the DesiredSize, just use the remaining space, proportionally
        /// </summary>
        public static readonly DependencyProperty GrowthProperty = DependencyProperty.RegisterAttached(
            "Growth", typeof(double), typeof(FlexPanel),
            new FrameworkPropertyMetadata(default(double),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static void SetGrowth(DependencyObject element, double value)
        {
            element.SetValue(GrowthProperty, value);
        }

        public static double GetGrowth(DependencyObject element)
        {
            return (double)element.GetValue(GrowthProperty);
        }

        //public static readonly DependencyProperty ConstraintProperty = DependencyProperty.RegisterAttached(
        //    "Constraint", typeof(double), typeof(FlexPanel), new PropertyMetadata(default(double)));

        //public static void SetConstraint(DependencyObject element, double value)
        //{
        //    element.SetValue(ConstraintProperty, value);
        //}

        //public static double GetConstraint(DependencyObject element)
        //{
        //    return (double) element.GetValue(ConstraintProperty);
        //}

        protected override Size MeasureOverride(Size constraint)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return Measure(constraint, Layout.Hor);
                case Orientation.Vertical:
                    return Measure(constraint, Layout.Ver);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Size Measure(Size constraint, ILayout layout)
        {
            Layout.CheckBreakOnMeasure(this);

            double vMax = 0;

            var uTotalSpacing = Math.Max(0, Spacing * (InternalChildren.Count - 1));
            var uAvailable = Math.Max(0, layout.GetU(constraint) - uTotalSpacing);
            var vAvailable = layout.GetV(constraint);
            var availableSize = layout.ToSize(uAvailable, vAvailable);

            double sumWeight = 0;
            double uSum = 0;

            foreach (UIElement child in InternalChildren)
            {
                var growth = GetGrowth(child);
                sumWeight += Math.Abs(growth);
                if (growth <= 0)
                {
                    child.Measure(availableSize);
                    var desiredSize = child.DesiredSize;
                    vMax = Math.Max(vMax, layout.GetV(desiredSize));
                    uSum += layout.GetU(desiredSize);
                }
            }

            var uRemaining = Math.Max(0, uAvailable - uSum);

            foreach (UIElement child in InternalChildren)
            {
                var growth = GetGrowth(child);
                if (growth > 0)
                {
                    var uSize = Math.Abs(growth) * uRemaining / sumWeight;
                    child.Measure(layout.ToSize(uSize, vAvailable));
                    var desiredSize = child.DesiredSize;
                    vMax = Math.Max(vMax, layout.GetV(desiredSize));
                    uSum += double.IsInfinity(uSize) ? layout.GetU(desiredSize) : uSize;
                }
            }

            return layout.ToSize(uSum + uTotalSpacing, vMax);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return Arrange(arrangeBounds, Layout.Hor);
                case Orientation.Vertical:
                    return Arrange(arrangeBounds, Layout.Ver);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Size Arrange(Size arrangeBounds, ILayout layout)
        {
            Layout.CheckBreakOnArrange(this);

            double uSpacing = 0;
            double spacing = Spacing;

            double uDesired = 0;
            double totalFlexWeight = 0;
            foreach (UIElement child in InternalChildren)
            {
                var growth = GetGrowth(child);
                uDesired += (growth <= 0 ? layout.GetU(child.DesiredSize) : 0) + uSpacing;
                totalFlexWeight += Math.Abs(growth);
                uSpacing = spacing;
            }

            var uRemaining = Math.Max(0, layout.GetU(arrangeBounds) - uDesired);

            double cursor = 0;

            uSpacing = 0;

            foreach (UIElement child in InternalChildren)
            {
                cursor += uSpacing;
                var growth = GetGrowth(child);
                var stretch = totalFlexWeight <= 0 ? 0 : uRemaining * Math.Abs(growth) / totalFlexWeight;
                var uBase = growth <= 0 ? layout.GetU(child.DesiredSize) : 0;
                double uSize = Math.Max(0, uBase + stretch);
                child.Arrange(layout.ToRect(cursor, 0, uSize, layout.GetV(arrangeBounds)));
                cursor += uSize;
                uSpacing = spacing;
            }

            return arrangeBounds;
            //return new Size(cursor, arrangeBounds.Height);
        }
    }
}