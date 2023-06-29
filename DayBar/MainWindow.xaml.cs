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
		int c = 0;

		// Get the current time
		DateTime now = DateTime.Now;
		// Get the start of the day
		DateTime startOfDay = now.Date;
		// Get the difference as a TimeSpan
		TimeSpan elapsed = now - startOfDay;
		// Get the total number of seconds
		int seconds = (int)elapsed.TotalSeconds;
		c = seconds * 100 / (endHour - startHour);

		dispatcherTimer.Interval = TimeSpan.FromMinutes(1);
		dispatcherTimer.Tick += (o, e) =>
		{
			if (c > 100)
			{
				c = 100;
			}
			myNotifyIcon.IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/{c}.ico"));
		};
		if (c > 100)
		{
			c = 100;
		}
		myNotifyIcon.IconSource = new BitmapImage(new Uri($"pack://application:,,,/Assets/Icons/{c}.ico"));

		dispatcherTimer.Start();
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
