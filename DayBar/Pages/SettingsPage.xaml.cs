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
using DayBar.Enums;
using PeyrSharp.Env;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DayBar.Pages;
/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
	public SettingsPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		VersionTxt.Text = Global.Version;
		LangComboBox.SelectedIndex = (int)Global.Settings.Language;

		ThemeSelectedBorder = Global.Settings.Theme switch
		{
			Themes.Light => LightBorder,
			Themes.Dark => DarkBorder,
			_ => SystemBorder
		};

		Border_MouseEnter(Global.Settings.Theme switch
		{
			Themes.Light => LightBorder,
			Themes.Dark => DarkBorder,
			_ => SystemBorder
		}, null);

		DarkRadio.IsChecked = Global.Settings.UseDarkThemeSystemTray;
		LightRadio.IsChecked = !Global.Settings.UseDarkThemeSystemTray;
	}

	private async void CheckUpdateBtn_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			string lastVersion = await Update.GetLastVersionAsync(Global.LastVersionLink);
			if (Update.IsAvailable(Global.Version, lastVersion))
			{
				UpdateTxt.Text = Properties.Resources.UpdateAvailable;
				if (MessageBox.Show(Properties.Resources.AvailableUpdates, $"{Properties.Resources.InstallVersion} {lastVersion}", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
				{
					return;
				}

				// If the user wants to proceed.
				SettingsManager.Save();

				Sys.ExecuteAsAdmin(Directory.GetCurrentDirectory() + @"\Xalyus Updater.exe"); // Start the updater
				Application.Current.Shutdown(); // Close
			}
		}
		catch { }
	}

	private void SeeLicensesBtn_Click(object sender, RoutedEventArgs e)
	{
		MessageBox.Show($"{Properties.Resources.Licenses}\n\n" +
			"Fluent System Icons - MIT License - © 2020 Microsoft Corporation\n" +
			"NotifyIcon.Wpf - The Code Project Open License (CPOL) 1.02 - © Hardcodet\n" +
			"PeyrSharp - MIT License - © 2022-2023 Devyus\n" +
			"DayBar - MIT License - © 2023 Léo Corporation", $"{Properties.Resources.DayBar} - {Properties.Resources.Licenses}", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void LangApplyBtn_Click(object sender, RoutedEventArgs e)
	{
		Global.Settings.Language = (Languages)LangComboBox.SelectedIndex;
		SettingsManager.Save();
		LangApplyBtn.Visibility = Visibility.Collapsed;

		if (MessageBox.Show(Properties.Resources.NeedRestartToApplyChanges, Properties.Resources.Settings, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
		{
			return;
		}
		Process.Start(Directory.GetCurrentDirectory() + @"\DayBar.exe");
		Application.Current.Shutdown();
	}

	private void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		LangApplyBtn.Visibility = Visibility.Visible;
	}

	Border ThemeSelectedBorder;
	private void Border_MouseEnter(object sender, MouseEventArgs e)
	{
		((Border)sender).BorderBrush = Global.GetSolidColor("Accent");
	}

	private void Border_MouseLeave(object sender, MouseEventArgs e)
	{
		if ((Border)sender == ThemeSelectedBorder) return;
		((Border)sender).BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
	}

	private void ResetBorders()
	{
		LightBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
		DarkBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
		SystemBorder.BorderBrush = new SolidColorBrush { Color = Colors.Transparent };
	}

	private void LightBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ResetBorders();
		ThemeSelectedBorder = (Border)sender;
		((Border)sender).BorderBrush = Global.GetSolidColor("Accent");

		Global.Settings.Theme = Themes.Light;
		SettingsManager.Save();
		Global.ChangeTheme();
	}

	private void DarkBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ResetBorders();
		ThemeSelectedBorder = (Border)sender;
		((Border)sender).BorderBrush = Global.GetSolidColor("Accent");

		Global.Settings.Theme = Themes.Dark;
		SettingsManager.Save();
		Global.ChangeTheme();

	}

	private void SystemBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ResetBorders();
		ThemeSelectedBorder = (Border)sender;
		((Border)sender).BorderBrush = Global.GetSolidColor("Accent");
		Global.Settings.Theme = Themes.System;
		SettingsManager.Save();
		Global.ChangeTheme();
	}

	private void RadioButton_Checked(object sender, RoutedEventArgs e)
	{
		try
		{
			Global.Settings.UseDarkThemeSystemTray = DarkRadio.IsChecked ?? false;
			Global.MainWindow.InitTimer(new(Global.Settings.StartHour, 0, 0), new(Global.Settings.EndHour, 0, 0));
			SettingsManager.Save();
		}
		catch { }
	}
}
