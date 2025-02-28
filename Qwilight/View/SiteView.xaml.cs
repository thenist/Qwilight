﻿using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SiteView
    {
        public SiteView()
        {
            InitializeComponent();
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as SiteViewModel).OnInputLower(e);

        void OnEssentialInputLower(object sender, KeyEventArgs e) => _ = (DataContext as SiteViewModel).OnEssentialInputLower(e);

        void OnPointedModified(object sender, KeyboardFocusChangedEventArgs e) => (DataContext as SiteViewModel).OnPointedModified(sender == e.NewFocus);

        void OnSiteYellsViewerMove(object sender, ScrollChangedEventArgs e) => (DataContext as SiteViewModel).OnSiteYellsViewerMove(e);

        void OnSetFavorNoteFile(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnSetFavorNoteFile();

        void OnSetFavorModeComponent(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnSetFavorModeComponent();

        void OnSetFavorAudioMultiplier(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnSetFavorAudioMultiplier();

        void OnSetAutoSiteHand(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnSetAutoSiteHand();

        void OnInputPostableItem(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnInputPostableItem();

        void OnAvatarGroup(object sender, EventArgs e) => (DataContext as SiteViewModel).OnAvatarGroup();

        void OnSetPostableItemBand(object sender, RoutedEventArgs e) => (DataContext as SiteViewModel).OnSetPostableItemBand();
    }
}