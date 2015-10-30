using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PlayStation_App.UserControls
{
    public sealed partial class PullToRefresh : UserControl
    {
        public PullToRefresh()
        {
            this.InitializeComponent();
        }

        public double PullProgress
        {
            get { return (double)GetValue(PullProgressProperty); }
            set { SetValue(PullProgressProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PullProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PullProgressProperty =
            DependencyProperty.Register("PullProgress", typeof(double), typeof(PullToRefresh), new PropertyMetadata(0, (o, p) =>
            {
                var ptr = o as PullToRefresh;
                if (ptr != null)
                {
                    var percentProgress = (double)p.NewValue;
                    var rotationAmount = Math.Min(percentProgress * 180, 180);
                    ptr.IconPanel.RenderTransform = new RotateTransform
                    {
                        Angle = rotationAmount
                    };

                    VisualStateManager.GoToState(ptr, (percentProgress >= 1) ? "ReleaseToRefresh" : "Normal", true);
                }
            }));

        public Style SymbolStyle
        {
            get { return (Style)GetValue(SymbolStyleProperty); }
            set { SetValue(SymbolStyleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for SymbolStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolStyleProperty =
            DependencyProperty.Register("SymbolStyle", typeof(Style), typeof(PullToRefresh), new PropertyMetadata(null));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TextStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register("TextStyle", typeof(Style), typeof(PullToRefresh), new PropertyMetadata(null));
    }
}
