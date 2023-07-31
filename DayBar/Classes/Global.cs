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
using DayBar.Enums;
using DayBar.Pages;
using Microsoft.Win32;
using PeyrSharp.Enums;
using PeyrSharp.Env;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DayBar.Classes
{
	public static class Global
	{
		public static string Version => "1.1.0.2308-pre1";
		public static string LastVersionLink => "https://raw.githubusercontent.com/Leo-Corporation/LeoCorp-Docs/master/Liens/Update%20System/DayBar/Version.txt";
		public static HomePage HomePage { get; set; }
		public static NotificationsPage NotificationsPage { get; set; }
		public static AboutPage AboutPage { get; set; }
		public static ThemePage ThemePage { get; set; }
		public static Settings Settings { get; set; }
		public static MainWindow MainWindow { get; set; }

		public static string GetHiSentence
		{
			get
			{
				if (DateTime.Now.Hour >= 21 && DateTime.Now.Hour <= 7) // If between 9PM & 7AM
				{
					return Properties.Resources.GoodNight + ", " + Environment.UserName + "."; // Return the correct value
				}
				else if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 12) // If between 7AM - 12PM
				{
					return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
				}
				else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 17) // If between 12PM - 5PM
				{
					return Properties.Resources.GoodAfternoon + ", " + Environment.UserName + "."; // Return the correct value
				}
				else if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 21) // If between 5PM - 9PM
				{
					return Properties.Resources.GoodEvening + ", " + Environment.UserName + "."; // Return the correct value
				}
				else
				{
					return Properties.Resources.Hi + ", " + Environment.UserName + "."; // Return the correct value
				}
			}
		}
		public static SolidColorBrush GetSolidColor(string resource) => (SolidColorBrush)Application.Current.Resources[resource];

		public static void ChangeTheme()
		{
			App.Current.Resources.MergedDictionaries.Clear();
			ResourceDictionary resourceDictionary = new(); // Create a resource dictionary

			bool isDark = Settings.Theme == Themes.Dark;
			if (Settings.Theme == Themes.System)
			{
				isDark = IsSystemThemeDark(); // Set
			}

			if (isDark) // If the dark theme is on
			{
				resourceDictionary.Source = new Uri("..\\Themes\\Dark.xaml", UriKind.Relative); // Add source
			}
			else
			{
				resourceDictionary.Source = new Uri("..\\Themes\\Light.xaml", UriKind.Relative); // Add source
			}

			App.Current.Resources.MergedDictionaries.Add(resourceDictionary); // Add the dictionary
		}

		public static bool IsSystemThemeDark()
		{
			if (Sys.CurrentWindowsVersion != WindowsVersion.Windows10 && Sys.CurrentWindowsVersion != WindowsVersion.Windows11)
			{
				return false; // Avoid errors on older OSs
			}

			var t = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", "1");
			return t switch
			{
				0 => true,
				1 => false,
				_ => false
			}; // Return
		}

		public static void ChangeLanguage()
		{
			switch (Settings.Language) // For each case
			{
				case Languages.Default: // No language
					break;
				case Languages.en_US: // English (US)
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US"); // Change
					break;

				case Languages.fr_FR: // French (FR)
					Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR"); // Change
					break;
				default: // No language
					break;
			}
		}
	}
}
