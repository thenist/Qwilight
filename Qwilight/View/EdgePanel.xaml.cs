using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Web.WebView2.Wpf;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    [INotifyPropertyChanged]
    public partial class EdgePanel
    {
        public static readonly DependencyProperty InitWwwProperty = DependencyProperty.Register(nameof(InitWww), typeof(string), typeof(EdgePanel));

        bool _isEdgeViewLoading;
        string _www = string.Empty;

        public string InitWww
        {
            get => GetValue(InitWwwProperty) as string;

            set => SetValue(InitWwwProperty, value);
        }

        public bool IsEdgeViewLoading
        {
            get => _isEdgeViewLoading;

            set => SetProperty(ref _isEdgeViewLoading, value, nameof(IsEdgeViewLoading));
        }

        public string Www
        {
            get => _www;

            set => SetProperty(ref _www, value, nameof(Www));
        }

        public EdgePanel()
        {
            InitializeComponent();
            EdgeView.CoreWebView2InitializationCompleted += (sender, e) => (sender as WebView2).CoreWebView2.Profile.DefaultDownloadFolderPath = QwilightComponent.EdgeEntryPath;
            EdgeView.NavigationStarting += (sender, e) => IsEdgeViewLoading = true;
            EdgeView.NavigationCompleted += (sender, e) => IsEdgeViewLoading = false;
            Loaded += OnInit;
        }

        void OnMove0(object sender, RoutedEventArgs e)
        {
            EdgeView.GoBack();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            EdgeView.Reload();
        }

        void OnInit(object sender, RoutedEventArgs e)
        {
            Www = InitWww;
            try
            {
                EdgeView.Source = new Uri(Www);
            }
            catch
            {
            }
        }

        void OnInputLower(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    EdgeView.Source = new Uri(Www);
                }
                catch
                {
                }
            }
        }
    }
}
