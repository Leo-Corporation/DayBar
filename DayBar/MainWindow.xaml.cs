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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
		RefreshNotifications();
		InitTimer(new(Global.Settings.StartHour, Global.Settings.StartMinute ?? 0, 0), new(Global.Settings.EndHour, Global.Settings.EndMinute ?? 0, 0));
		InitUI();
		Hide();
	}
	List<int> percentages = new List<int>();
	List<bool> shown = new List<bool>();
	internal void RefreshNotifications()
	{
		percentages.Clear();
		shown.Clear();
		// Calculate the total number of notifications required (100 / NotifyPercentageValue)
		int totalNotifications = 100 / Global.Settings.NotifyPercentageValue.GetValueOrDefault(25);

		// Fill the percentages list with the desired percentages for notifications
		for (int i = 0; i < totalNotifications; i++)
		{
			int percentage = (i + 1) * Global.Settings.NotifyPercentageValue.GetValueOrDefault(25);
			percentages.Add(percentage);
			shown.Add(false);
		}
	}

	string lastVersion = "";
	private async void InitUI()
	{
		HelloTxt.Text = Global.GetHiSentence;

		try
		{
			lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink);
			if (Update.IsAvailable(Global.Version, lastVersion))
			{
				UpdatesAvailable();
			}
		}
		catch { }
		HomeBtn.IsChecked = true;
	}

	DispatcherTimer dispatcherTimer = new();
	bool halfShown = false;

	internal void InitTimer(TimeSpan startWorkHour, TimeSpan endWorkHour)
	{
		dispatcherTimer.Stop();
		dispatcherTimer = new();
		int c = CalculatePercentage(DateTime.Now, startWorkHour, endWorkHour);

		dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
		dispatcherTimer.Tick += (o, e) =>
		{
			c = CalculatePercentage(DateTime.Now, startWorkHour, endWorkHour);
			SetNotifyIcon(ref c);
		};

		SetNotifyIcon(ref c);
		dispatcherTimer.Start();
	}

	private int CalculatePercentage(DateTime currentTime, TimeSpan startWorkHour, TimeSpan endWorkHour)
	{
		// Get the current date and time
		DateTime currentDate = DateTime.Now;

		// Create DateTime objects for the start and end work hours
		DateTime startDateTime = currentDate.Date.Add(startWorkHour);
		DateTime endDateTime = currentDate.Date.Add(endWorkHour);

		// If end work hour is before the start work hour, adjust it to the next day
		if (endDateTime < startDateTime)
		{
			endDateTime = endDateTime.AddDays(1);
		}

		// Check if the current time is within the range of the previous day
		if (currentTime < startDateTime)
		{
			startDateTime = startDateTime.AddDays(-1);
			endDateTime = endDateTime.AddDays(-1);
		}

		// Calculate the total number of minutes between start and end work hours
		TimeSpan totalWorkHours = endDateTime - startDateTime;
		int totalWorkMinutes = (int)totalWorkHours.TotalMinutes;

		// Calculate the number of minutes passed since the start of the workday
		TimeSpan timePassed = currentTime - startDateTime;
		int minutesPassed = (int)timePassed.TotalMinutes;

		// Calculate the percentage of time passed (rounded down to the nearest integer)
		int percentagePassed = (int)((double)minutesPassed / totalWorkMinutes * 100);

		// Make sure the percentage is within the range of 0 to 100
		percentagePassed = Math.Max(0, Math.Min(100, percentagePassed));

		return percentagePassed;
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

		// Notifications
		if (!Global.Settings.NotificationDays.Value.IsNotificationDay()) return;
		if (!halfShown && Global.Settings.NotifyHalfDay && progress >= 50)
		{
			halfShown = true;
			myNotifyIcon.ShowBalloonTip(Properties.Resources.DayBar, Properties.Resources.HalfPassed, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
		}

		if (Global.Settings.NotifyPercentage ?? false)
		{
			// Show notifications when progress reaches the specified percentages
			for (int i = percentages.Count - 1; i >= 0; i--)
			{
				if (progress >= percentages[i] && !shown[i] && !shown[^1])
				{
					shown[i] = true;
					for (int j = i - 1; j >= 0; j--)
					{
						shown[j] = true;
					}
					myNotifyIcon.ShowBalloonTip(Properties.Resources.DayBar, string.Format(Properties.Resources.XDayHasPassed, progress), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
					break; // To show only one notification per loop
				}
			}
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



	private void HomeBtn_Click(object sender, RoutedEventArgs e)
	{
		PageContent.Navigate(Global.HomePage);
	}

	private void SettingsMenu_Click(object sender, RoutedEventArgs e)
	{
		PageContent.Navigate(Global.HomePage);
		Show();
	}

	private void AboutMenu_Click(object sender, RoutedEventArgs e)
	{
		PageContent.Navigate(Global.AboutPage);
		Show();
	}

	private void QuitMenu_Click(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown(); // Close the application
	}

	private void SettingsBtn_Click(object sender, RoutedEventArgs e)
	{

	}
}
