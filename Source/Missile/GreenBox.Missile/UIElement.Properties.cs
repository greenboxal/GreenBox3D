using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox.Missile
{
    public partial class UIElement
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility",
            typeof(Visibility),
            typeof(UIElement),
            new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
    }
}
