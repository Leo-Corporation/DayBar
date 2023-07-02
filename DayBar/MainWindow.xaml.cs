/*
MIT License

Copyright (c) Léo Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/
using DayBar.Classes;
using PeyrSharp.Env;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DayBar;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		Global.MainWindow = this;
		InitTimer(Global.Settings.StartHour * 3600, Global.Settings.EndHour * 3600);
		InitUI();
		Hide();
	}

	string lastVersion = "";
	private async void InitUI()
	{
		HelloTxt.Text = Global.GetHiSentence;

		UnCheckAll();
		CheckButton(HomeBtn);
		PageContent.Navigate(Global.HomePage);
		try
		{
			lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink);
			if (Update.IsAvailable(Global.Version, lastVersion))
			{
				UpdatesAvailable();
			}
		}
		catch { }
	}

	DispatcherTimer dispatcherTimer = new();
	bool halfShown = false;
	internal void InitTimer(int startHour, int endHour)
	{
		dispatcherTimer.Stop();
		int c = CalculatePercentage(startHour, endHour);

		dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
		dispatcherTimer.Tick += (o, e) =>
		{
			c = CalculatePercentage(startHour, endHour);
			SetNotifyIcon(ref c);
		};

		SetNotifyIcon(ref c);
		dispatcherTimer.Start();
	}

	private int CalculatePercentage(int startHour, int endHour)
	{
		// Get the current time
		DateTime now = DateTime.Now;
		// Get the start of the day
		DateTime startOfDay = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour / 3600, 0, 0);
		// Get the difference as a TimeSpan
		TimeSpan elapsed = now - startOfDay;
		// Get the total number of seconds
		int seconds = (int)elapsed.TotalSeconds;
		return seconds * 100 / (endHour - startHour);
	}

	private void SetNotifyIcon(ref int progress)
	{
		// Avoid errors
		if (progress > 100) progress = 100;
		if (progress < 0) progress = 0;

		myNotifyIcon.IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/{progress}{(Global.Settings.UseDarkThemeSystemTray ? "" : "L")}.ico"));
		myNotifyIcon.ToolTipText = $"{Properties.Resources.DayBar} ({progress}%)";

		Global.HomePage.ProgressTxt.Text = $"{progress}%";
		Global.HomePage.ProgressBar.Value = progress;

		Global.ThemePage.LightProgressTxt.Text = $"{progress}%";
		Global.ThemePage.DarkProgressTxt.Text = $"{progress}%";

		if (!halfShown && Global.Settings.NotifyHalfDay)
		{
			halfShown = true;
			myNotifyIcon.ShowBalloonTip(Properties.Resources.DayBar, Properties.Resources.HalfPassed, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
		}
	}

	internal void UpdatesAvailable()
	{
		if (!Global.Settings.NotifyUpdate) return;

		void BalloonClick(object o, RoutedEventArgs e)
		{
			myNotifyIcon.TrayBalloonTipClosed += (o, e) =>
			{
				myNotifyIcon.TrayBalloonTipClicked -= BalloonClick;
			};
			if (MessageBox.Show(Properties.Resources.AvailableUpdates, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
			{
				return;
			}

			// If the user wants to proceed.
			SettingsManager.Save();

			Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
			Application.Current.Shutdown(); // Close
			
		}

		myNotifyIcon.TrayBalloonTipClicked += BalloonClick;
		myNotifyIcon.ShowBalloonTip(Properties.Resources.DayBar, Properties.Resources.AvailableUpdates, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized;
	}

	private void UnCheckAll()
	{
		HomeBtn.Background = Brushes.Transparent;
		NotificationsBtn.Background = Brushes.Transparent;
		ThemeBtn.Background = Brushes.Transparent;
		SettingsBtn.Background = Brushes.Transparent;

		HomeBtn.BorderBrush = Brushes.Transparent;
		NotificationsBtn.BorderBrush = Brushes.Transparent;
		ThemeBtn.BorderBrush = Brushes.Transparent;
		SettingsBtn.BorderBrush = Brushes.Transparent;
	}

	internal void CheckButton(Button btn)
	{
		UnCheckAll();
		btn.Background = Global.GetSolidColor("Background2");
		btn.BorderBrush = Global.GetSolidColor("Accent");
	}

	private void HomeBtn_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(HomeBtn);
		PageContent.Navigate(Global.HomePage);
	}

	private void NotificationsBtn_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(NotificationsBtn);
		PageContent.Navigate(Global.NotificationsPage);
	}

	private void SettingsBtn_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(SettingsBtn);
		PageContent.Navigate(Global.AboutPage);
	}

	private void SettingsMenu_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(HomeBtn);
		PageContent.Navigate(Global.HomePage);
		Show();
	}

	private void AboutMenu_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(SettingsBtn);
		PageContent.Navigate(Global.AboutPage);
		Show();
	}

	private void QuitMenu_Click(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown(); // Close the application
	}

	private void ThemeBtn_Click(object sender, RoutedEventArgs e)
	{
		CheckButton(ThemeBtn);
		PageContent.Navigate(Global.ThemePage);
	}
}
