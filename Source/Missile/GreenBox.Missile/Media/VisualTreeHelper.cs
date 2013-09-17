using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile.Media
{
    public static class VisualTreeHelper
    {
        public static DependencyObject GetParent(DependencyObject reference)
        {
            Visual visual = reference as Visual;

            if (visual == null)
                return null;

            return visual.GetParent();
        }

        public static UIElement GetUIParent(UIElement reference)
        {
            Visual v = null;

            for (v = (Visual)GetParent(reference); v != null; v = (Visual)GetParent(v))
                if (v is UIElement)
                    break;

            return (UIElement)v;
        }
    }
}
