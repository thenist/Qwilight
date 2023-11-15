using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Igniter.MSG;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Igniter.ViewModel
{
    public sealed partial class MainViewModel : ObservableObject
    {
        double _value;
        string _text;
        bool _isVisible = true;

        public double Value
        {
            get => _value;

            set => SetProperty(ref _value, value, nameof(Value));
        }

        public string Text
        {
            get => _text;

            set => SetProperty(ref _text, value, nameof(Text));
        }

        public bool IsVisible
        {
            get => _isVisible;

            set => SetProperty(ref _isVisible, value, nameof(IsVisible));
        }

        public async Task OnLoaded()
        {
            switch (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.Levy,
                Input = MessageBoxButton.OKCancel,
                Drawing = MessageBoxImage.Information
            }).Response)
            {
                case MessageBoxResult.OK:
                    while (true)
                    {
                        try
                        {
                            using (var zipFile = ZipFile.Read(Environment.GetCommandLineArgs()[1]))
                            {
                                zipFile.ExtractProgress += (sender, e) =>
                                {
                                    if (e.EntriesTotal > 0)
                                    {
                                        Value = 100.0 * e.EntriesExtracted / e.EntriesTotal;
                                    }
                                    var fileName = e.CurrentEntry?.FileName;
                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        Text = fileName;
                                    }
                                };
                                await Task.Run(() => zipFile.ExtractAll(Path.GetDirectoryName(IgniterComponent.QwilightFilePath), ExtractExistingFileAction.OverwriteSilently)).ConfigureAwait(false);
                            }

                            OnIgnited();
                        }
                        catch (Exception e)
                        {
                            OnIgnitingFault(e);
                            if (IsVisible)
                            {
                                continue;
                            }
                        }
                    }
                case MessageBoxResult.Cancel:
                    Environment.Exit(1);
                    break;
            }

            void OnIgnited()
            {
                IsVisible = false;
                StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                {
                    Text = LanguageSystem.Instance.Ignited,
                    Input = MessageBoxButton.OK,
                    Drawing = MessageBoxImage.Information
                });
                switch (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                {
                    Text = LanguageSystem.Instance.ExeQwilight,
                    Input = MessageBoxButton.YesNo,
                    Drawing = MessageBoxImage.Question
                }).Response)
                {
                    case MessageBoxResult.Yes:
                        Process.Start(IgniterComponent.QwilightFilePath);
                        break;
                }
                Environment.Exit(0);
            }

            void OnIgnitingFault(Exception e)
            {
                IsVisible = false;
                StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                {
                    Text = e.Message,
                    Input = MessageBoxButton.OK,
                    Drawing = MessageBoxImage.Error
                });
                switch (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
                {
                    Text = LanguageSystem.Instance.IgnitingFault,
                    Input = MessageBoxButton.YesNo,
                    Drawing = MessageBoxImage.Warning
                }).Response)
                {
                    case MessageBoxResult.Yes:
                        IsVisible = true;
                        break;
                    case MessageBoxResult.No:
                        Environment.Exit(1);
                        break;
                }
            }
        }
    }
}