using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

// Avalonia app that creates our window for configuring the IP address
namespace Quest_Discord_Presence_Client
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new ConfigWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
