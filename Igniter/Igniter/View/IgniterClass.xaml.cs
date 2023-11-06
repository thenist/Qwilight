using System;
using System.Windows;

namespace Igniter.View
{
    public partial class IgniterClass
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IgniterComponent.OnGetBuiltInData = data => TryFindResource(data);

            if (e.Args.Length == 0)
            {
                MessageBox.Show(LanguageSystem.Instance.LevyFault, "Qwilight", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(1);
            }
        }
    }
}
