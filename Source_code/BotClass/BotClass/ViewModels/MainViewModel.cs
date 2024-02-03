using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;
using System.Threading;
using BotClass.Views;
using System;
using DynamicData.Binding;
using System.Diagnostics;
using System.Threading.Tasks;


namespace BotClass.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ReactiveCommand<Unit,Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowSettings { get; }
    public ReactiveCommand<Unit, Unit> ShowAppFromTray { get; }
    public ReactiveCommand<Unit, Unit> PushMessage { get; }

    public MainViewModel()
    {
        ExitCommand = ReactiveCommand.Create(ExitMethod);
        ShowSettings = ReactiveCommand.Create(ShowSettingsMethod);
        ShowAppFromTray = ReactiveCommand.Create(ShowAppFromTrayMethod);
        PushMessage = ReactiveCommand.Create(PushMessageMethod);

        IsStartingWithSystem = Properties.Settings.Default.StartWithSystem;
        Token = Properties.Settings.Default.Token;
        AlwaysMinimazeToTray = Properties.Settings.Default.AlwaysMinimazeToTray;
        Properties.Settings.Default.GointExitFromTray = false;

        BotClassWrapper.ResponseReceived += (s, e) =>
        {
            Result += e.Response;
            var window = ((App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow as MainWindow);
            window.ProgressBar.IsIndeterminate = false;
        };
    }

    #region Methods
    private void PushMessageMethod()
    {
        var window = ((App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow as MainWindow);
        window.TextBoxResult.Text = "";
        window.ProgressBar.IsIndeterminate = true;
        IsIndeterminateProgressBar = true;
        _ = BotClassWrapper.PushMessage(Properties.Settings.Default.Token, Request);
        
    }

    private void ShowAppFromTrayMethod()
    {
        var window = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        window.ShowInTaskbar = true;
        window.WindowState = Avalonia.Controls.WindowState.Normal;
        window.Activate();
        
    }
    private void ExitMethod()
    {
        Properties.Settings.Default.GointExitFromTray = true;
        var window = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        SettingsWindowInstance?.Close();
        window?.Close();
    }

    private void ShowSettingsMethod()
    {
        if(SettingsWindowInstance == null)
        {
            SettingsWindowInstance = new SettingsWindow();
        }

        try
        {
            SettingsWindowInstance.Show();
            SettingsWindowInstance.Activate();
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException)
            {
                SettingsWindowInstance?.Close();
                SettingsWindowInstance = new SettingsWindow();
                SettingsWindowInstance.Show();
            }
        }


        
    }
    #endregion
    public Avalonia.Controls.Window SettingsWindowInstance = null;
    public Avalonia.Controls.Window MainWindowInstance = null;
    public bool IsStartingWithSystem { get; set; } = false;
    public bool AlwaysMinimazeToTray { get; set; } = true;
    public bool IsIndeterminateProgressBar { get; set; } = false;
    public string Token { get; set; } = "";
    public string Request { get; set; } = "";
    public string Result { get; set; } = "";


    //public string Greeting => "Welcome to Avalonia!";
}
