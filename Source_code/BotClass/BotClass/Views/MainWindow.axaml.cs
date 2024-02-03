using Avalonia.Controls;
using ReactiveUI;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using BotClass.ViewModels;
using Avalonia.Data;
using System.Diagnostics;
using Avalonia.Input;
using System.Reflection;

using System.Runtime.InteropServices;
using static BotClass.Views.MainWindow;
using System.Linq;
using System.Threading;
using SharpHook;
using System.Collections.Generic;
using SharpHook.Native;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Avalonia.Input.Platform;
using Splat;
using Avalonia.Controls.Platform;
using Avalonia.Rendering;




#if OS_WINDOWS



#endif

namespace BotClass.Views;

public partial class MainWindow : Window
{
    TaskPoolGlobalHook hook = new TaskPoolGlobalHook();
    SortedSet<SharpHook.Native.KeyCode> keyCodesPressed = new SortedSet<SharpHook.Native.KeyCode>();
    public MainWindow()
    {
        InitializeComponent();
        
        // Binding events
        CrossDelText.PointerEntered += CrossDelText_PointerEntered;
        CrossDelText.PointerExited += CrossDelText_PointerExited;
        CrossDelText.Tapped += CrossDelText_Tapped;
        this.Closing += MainWindow_Closing;
        this.Resized += MainWindow_Resized;
        EnterText.SizeChanged += EnterText_SizeChanged;

        BotClassWrapper.ResponseReceived += (s, e) => 
        {
            TextBoxResult.Text = e.Response ?? "";
        };
        EnterText.KeyDown += EnterText_KeyDown;

        DataContext = new MainViewModel();

        

        hook.KeyPressed += (s, e) =>
        {
            if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcF2 || e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftControl)
            {
                keyCodesPressed.Add(e.Data.KeyCode);
                
            }

            if(keyCodesPressed.Count == 2) //Ctrl+F2 was pressed
            {
                keyCodesPressed.Clear();
                var saveOldClipboard = Clipboard.GetTextAsync();
                saveOldClipboard.Wait();
                var TaskClear = Clipboard.ClearAsync();
                TaskClear.Wait();
                var simulator = new EventSimulator();
                simulator.SimulateKeyRelease(KeyCode.VcF2);

                if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // if OS is Windows
                {
                    // Press Ctrl+C
                    simulator.SimulateKeyPress(KeyCode.VcLeftControl);
                    simulator.SimulateKeyPress(KeyCode.VcC);

                    // Release Ctrl+C
                    simulator.SimulateKeyRelease(KeyCode.VcC);
                    simulator.SimulateKeyRelease(KeyCode.VcLeftControl);
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // if OS is MacOS
                {
                    // Press Comma+C for MacOS
                    simulator.SimulateKeyPress(KeyCode.VcLeftMeta);
                    simulator.SimulateKeyPress(KeyCode.VcC);
                    // Release Comma+C
                    simulator.SimulateKeyRelease(KeyCode.VcLeftMeta);
                    simulator.SimulateKeyRelease(KeyCode.VcC);
                }

                    
                Thread.Sleep(500);

                var copyText = Clipboard.GetTextAsync();
                copyText.Wait();
                Clipboard.SetTextAsync(saveOldClipboard.Result);
                Dispatcher.UIThread.InvokeAsync(new Action(() =>
                {
                    var window = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                    window.ShowInTaskbar = true;
                    window.WindowState = Avalonia.Controls.WindowState.Normal;
                    window.Activate();
                    (window as MainWindow).EnterText.Text = copyText.Result;
                    (window as MainWindow).PushMessageButton.Focus();
                })).Wait();
                
                
                simulator.SimulateKeyPress(KeyCode.VcEnter);
                simulator.SimulateKeyRelease(KeyCode.VcEnter);

                
            }
        };
        hook.KeyReleased += (s, e) =>
        {
            if(keyCodesPressed.Contains(e.Data.KeyCode))
            {
                keyCodesPressed.Remove(e.Data.KeyCode);
            }
        };

        Task.Factory.StartNew(() =>
        {
            hook.Run();
        });

        

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            
        }

        //HotKeyManager.SetHotKey(PushMessageButton, new Avalonia.Input.KeyGesture(Avalonia.Input.Key.Space, Avalonia.Input.KeyModifiers.Control));
    }

    // Resize rectangle when entertext resized
    private void EnterText_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        RectangleBackroungOfEnterText.Height = EnterText.Height;
        //CrossDelText.Margin = new Thickness(-30, 0, 0, 10);
    }

    private void EnterText_KeyDown(object? sender, KeyEventArgs e)
    {
       
        
    }

    private void MainWindow_Resized(object? sender, WindowResizedEventArgs e)
    {
        if(WindowState == WindowState.Minimized && Properties.Settings.Default.AlwaysMinimazeToTray)
        {
            this.ShowInTaskbar = false;
        }
        EnterText.Width = this.Width - 140;
    }

        // Close the settings window when main window is closing
        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        var mV = (this.DataContext as MainViewModel);
        if (Properties.Settings.Default.AlwaysMinimazeToTray && Properties.Settings.Default.GointExitFromTray == false)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }
        
        mV?.SettingsWindowInstance?.Close();
    }

    // Clear the input textbox
    private void CrossDelText_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        EnterText.Text = "";
    }
    // Change image when mouse leave from cross
    private void CrossDelText_PointerExited(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var bitmap = new Bitmap(AssetLoader.Open(new Uri("avares://BotClass/Assets/Cross.png")));
        CrossDelText.Source = bitmap;
    }
    // Change image when mouse enter cross
    private void CrossDelText_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var bitmap = new Bitmap(AssetLoader.Open(new Uri("avares://BotClass/Assets/Cross_white.png")));
        CrossDelText.Source = bitmap;
    }

}
