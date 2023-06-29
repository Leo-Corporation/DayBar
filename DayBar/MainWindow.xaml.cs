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
using System;
using System.Collections.Generic;
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
		InitTimer(0*3600, 24*3600);
	}

	DispatcherTimer dispatcherTimer = new();
	
	private void InitTimer(int startHour, int endHour)
	{

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
		if (progress > 100)
		{
			progress = 100;
		}
		myNotifyIcon.IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/{progress}.ico"));
		myNotifyIcon.ToolTipText = $"{Properties.Resources.DayBar} ({progress}%)";
	}

	private void CloseBtn_Click(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown(); // Close the application
	}

	private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized;
	}
}
