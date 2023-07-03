﻿/*
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
using DayBar.Windows;
using System.Windows;
using System.Windows.Controls;

namespace DayBar.Pages.FirstRun
{
	/// <summary>
	/// Interaction logic for JumpIn.xaml
	/// </summary>
	public partial class JumpInPage : Page
	{
		FirstRunWindow FirstRunWindow { get; init; }
		public JumpInPage(FirstRunWindow firstRunWindow)
		{
			InitializeComponent();
			FirstRunWindow = firstRunWindow;
		}

		private void NextBtn_Click(object sender, RoutedEventArgs e)
		{
			Global.Settings.IsFirstRun = false;
			SettingsManager.Save();

			Global.MainWindow.Show();
			FirstRunWindow.Close();
		}
	}
}
