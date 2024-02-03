using Avalonia.Controls;
using BotClass.ViewModels;
using Microsoft.Win32;
using ReactiveUI;
using System.Reactive;

namespace BotClass.Views
{
    public partial class SettingsWindow : Window
    {


        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            TextBoxToken.TextChanged += TextBoxToken_TextChanged;
            CheckBoxStartWithSystem.IsCheckedChanged += CheckBoxStartWithSystem_IsCheckedChanged;
            CheckBoxStartAlwaysMin.IsCheckedChanged += CheckBoxStartAlwaysMin_IsCheckedChanged;
            //this.Closing += SettingsWindow_Closing;
        }

        private void CheckBoxStartAlwaysMin_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysMinimazeToTray = CheckBoxStartAlwaysMin.IsChecked ?? true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxStartWithSystem_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Properties.Settings.Default.StartWithSystem = CheckBoxStartWithSystem.IsChecked ?? false;
            Properties.Settings.Default.Save();
            /*
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (Properties.Settings.Default.StartWithSystem)
            {
                rk.SetValue(App.Current.Name, Path);
            }
            else
                rk.DeleteValue(AppName, false);*/
        }

        public void MakeAutoStart ()
        {
            

        }

        public void DeleteAutoStart()
        {

        }

        private void TextBoxToken_TextChanged(object? sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Token = TextBoxToken.Text;
            Properties.Settings.Default.Save();
        }

        private void SettingsWindow_Closing(object? sender, WindowClosingEventArgs e)
        {

        }
    }
}
