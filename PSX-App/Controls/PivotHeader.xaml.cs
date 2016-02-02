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

// Originally used in https://github.com/UWPanda/TabbedPivot/blob/master/TabbedPivot/TabbedPivot/PivotHeader.xaml.cs

namespace PlayStation_App.Controls
{
    public sealed partial class PivotHeader : UserControl
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PivotHeader), null);

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(PivotHeader), null);

        public string Icon
        {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        public PivotHeader()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
