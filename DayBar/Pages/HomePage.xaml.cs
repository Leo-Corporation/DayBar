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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
/// Interaction logic for HomePage.xaml
/// </summary>
public partial class HomePage : Page
{
	public HomePage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		FromTxt.Text = Global.Settings.StartHour.ToString();
		ToTxt.Text = Global.Settings.EndHour.ToString();
		LangComboBox.SelectedIndex = (int)Global.Settings.Language;
		LaunchOnStartChk.IsChecked = Global.Settings.LaunchOnStart;
	}

	private void ValidateTxt_Click(object sender, RoutedEventArgs e)
	{
		int start = int.Parse(FromTxt.Text);
		int end = int.Parse(ToTxt.Text);

		if (start < end && start >= 0 && end <= 24)
		{
			Global.Settings.StartHour = start;
			Global.Settings.EndHour = end;
			SettingsManager.Save();
			Global.MainWindow.InitTimer(start * 3600, end * 3600);

			return;
		}
		MessageBox.Show(Properties.Resources.WorkHoursError, Properties.Resources.DayBar, MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private void FromTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void CheckBox_Checked(object sender, RoutedEventArgs e)
	{
		var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
		key?.SetValue("DayBar", Environment.ProcessPath ?? $@"{AppContext.BaseDirectory}\DayBar.exe");
		Global.Settings.LaunchOnStart = LaunchOnStartChk.IsChecked ?? true;
		SettingsManager.Save();
	}

	private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
		key?.DeleteValue("DayBar", false);
		Global.Settings.LaunchOnStart = LaunchOnStartChk.IsChecked ?? true;
		SettingsManager.Save();
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
}
