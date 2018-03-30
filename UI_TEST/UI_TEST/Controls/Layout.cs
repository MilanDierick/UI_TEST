using System.Diagnostics;
using System.Windows;

namespace UI_TEST
{
    public interface ILayout
    {
        double GetU(Size size);
        double GetV(Size size);
        Size ToSize(double uSize, double vSize);
        Rect ToRect(double uPos, double vPos, double uSize, double vSize);
    }

    public sealed class HorLayout : ILayout
    {
        public double GetU(Size size)
        {
            return size.Width;
        }

        public double GetV(Size size)
        {
            return size.Height;
        }

        public Size ToSize(double uSize, double vSize)
        {
            return new Size(uSize, vSize);
        }

        public Rect ToRect(double uPos, double vPos, double uSize, double vSize)
        {
            return new Rect(uPos, vPos, uSize, vSize);
        }
    }

    public sealed class VerLayout : ILayout
    {
        public double GetU(Size size)
        {
            return size.Height;
        }

        public double GetV(Size size)
        {
            return size.Width;
        }

        public Size ToSize(double uSize, double vSize)
        {
            return new Size(vSize, uSize);
        }

        public Rect ToRect(double uPos, double vPos, double uSize, double vSize)
        {
            return new Rect(vPos, uPos, vSize, uSize);
        }
    }


    public static class Layout
    {
        public static readonly HorLayout Hor = new HorLayout();
        public static readonly VerLayout Ver = new VerLayout();

        public static readonly DependencyProperty BreakOnLayoutProperty = DependencyProperty.RegisterAttached(
            "BreakOnLayout", typeof(bool), typeof(Layout), new PropertyMetadata(default(bool)));

        public static void SetBreakOnLayout(DependencyObject element, bool value)
        {
            element.SetValue(BreakOnLayoutProperty, value);
        }

        public static bool GetBreakOnLayout(DependencyObject element)
        {
            return (bool)element.GetValue(BreakOnLayoutProperty);
        }

        public static readonly DependencyProperty BreakOnArrangeProperty = DependencyProperty.RegisterAttached(
            "BreakOnArrange", typeof(bool), typeof(Layout), new PropertyMetadata(default(bool)));

        public static void SetBreakOnArrange(DependencyObject element, bool value)
        {
            element.SetValue(BreakOnArrangeProperty, value);
        }

        public static bool GetBreakOnArrange(DependencyObject element)
        {
            return (bool)element.GetValue(BreakOnArrangeProperty);
        }

        public static readonly DependencyProperty BreakOnMeasureProperty = DependencyProperty.RegisterAttached(
            "BreakOnMeasure", typeof(bool), typeof(Layout), new PropertyMetadata(default(bool)));

        public static void SetBreakOnMeasure(DependencyObject element, bool value)
        {
            element.SetValue(BreakOnMeasureProperty, value);
        }

        public static bool GetBreakOnMeasure(DependencyObject element)
        {
            return (bool)element.GetValue(BreakOnMeasureProperty);
        }

        [Conditional("DEBUG")]
        public static void CheckBreakOnMeasure(DependencyObject element)
        {
            if (Debugger.IsAttached && (GetBreakOnLayout(element) || GetBreakOnMeasure(element)))
                Debugger.Break();
        }

        [Conditional("DEBUG")]
        public static void CheckBreakOnArrange(DependencyObject element)
        {
            if (Debugger.IsAttached && (GetBreakOnLayout(element) || GetBreakOnArrange(element)))
                Debugger.Break();
        }
    }
}