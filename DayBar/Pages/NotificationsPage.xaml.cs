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
using System.Windows;
using System.Windows.Controls;

namespace DayBar.Pages;
/// <summary>
/// Interaction logic for NotificationsPage.xaml
/// </summary>
public partial class NotificationsPage : Page
{
	public NotificationsPage()
	{
		InitializeComponent();
		InitUI();
	}

	private void InitUI()
	{
		NotifyUpdatesChk.IsChecked = Global.Settings.NotifyUpdate;
		NotifyHalfChk.IsChecked = Global.Settings.NotifyHalfDay;
	}

	private void NotifyUpdatesChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.NotifyUpdate = NotifyUpdatesChk.IsChecked ?? true;
		SettingsManager.Save();
	}

	private void NotifyHalfChk_Checked(object sender, RoutedEventArgs e)
	{
		Global.Settings.NotifyHalfDay = NotifyHalfChk.IsChecked ?? false;
		SettingsManager.Save();
	}
}
