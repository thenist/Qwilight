﻿using Qwilight.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SiteContainerWindow
    {
        public SiteContainerWindow() => InitializeComponent();

        void OnSiteView(object sender, SelectionChangedEventArgs e) => (DataContext as SiteContainerViewModel).OnSiteView();

        void OnPointLower(object sender, MouseButtonEventArgs e) => e.Handled = true;
    }
}