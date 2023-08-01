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
using PeyrSharp.Env;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DayBar.Classes
{
	public class Settings
	{
		/// <summary>
		/// The Working start hour.
		/// </summary>
		public int StartHour { get; set; }

		/// <summary>
		/// The Working end hour.
		/// </summary>
		public int EndHour { get; set; }

		/// <summary>
		/// <see langword="true"/> if the app should launch on start.
		/// </summary>
		public bool LaunchOnStart { get; set; }

		/// <summary>
		/// <see langword="true"/> if the icon displayed in the system should support dark theme taskbar.
		/// </summary>
		public bool UseDarkThemeSystemTray { get; set; }

		/// <summary>
		/// The theme of the app.
		/// </summary>
		public Themes Theme { get; set; }

		/// <summary>
		/// <see langword="true"/> if the app should display a banner when an update is available.
		/// </summary>
		public bool NotifyUpdate { get; set; }

		/// <summary>
		/// <see langword="true"/> if the app should display a banner when half of the day has been reached.
		/// </summary>
		public bool NotifyHalfDay { get; set; }

		/// <summary>
		/// The language of the app.
		/// </summary>
		public Languages Language { get; set; }

		/// <summary>
		/// <see langword="true"/> if this is the user is launching the app for the first time.
		/// </summary>
		public bool IsFirstRun { get; set; }

		/// <summary>
		/// The working hour start minute.
		/// </summary>
		public int? StartMinute { get; set; }

		/// <summary>
		/// The working hour end minute.
		/// </summary>
		public int? EndMinute { get; set; }

		/// <summary>
		/// True if the app sould show a notification every x%.
		/// </summary>
		public bool? NotifyPercentage { get; set; }

		/// <summary>
		/// The value to look when showing a notfication every x%.
		/// </summary>
		public int? NotifyPercentageValue { get; set; }
		public NotificationDays? NotificationDays { get; set; }

		public Settings()
		{
			// Default configuration
			StartHour = 0;
			StartMinute = 0;
			EndHour = 24;
			EndMinute = 0;
			NotifyUpdate = true;
			NotifyHalfDay = false;
			LaunchOnStart = true;
			UseDarkThemeSystemTray = false;
			Theme = Themes.System;
			Language = Languages.Default;
			IsFirstRun = true;
			NotifyPercentage = false;
			NotifyPercentageValue = 25;
			NotificationDays = new()
			{
				Monday = true,
				Tuesday = true,
				Wednesday = true,
				Thursday = true,
				Friday = true,
				Saturday = true,
				Sunday = true
			};
		}
	}

	public struct NotificationDays
	{
		public bool Monday { get; set; }
		public bool Tuesday { get; set; }
		public bool Wednesday { get; set; }
		public bool Thursday { get; set; }
		public bool Friday { get; set; }
		public bool Saturday { get; set; }
		public bool Sunday { get; set; }

		public bool IsNotificationDay()
		{
			return DateTime.Now.DayOfWeek switch
			{
				DayOfWeek.Monday => Monday,
				DayOfWeek.Tuesday => Tuesday,
				DayOfWeek.Wednesday => Wednesday,
				DayOfWeek.Thursday => Thursday,
				DayOfWeek.Friday => Friday,
				DayOfWeek.Saturday => Saturday,
				DayOfWeek.Sunday => Sunday,
				_ => true
			};
		}
	}

	public static class SettingsManager
	{
		private static string SettingsPath => $@"{FileSys.AppDataPath}\Léo Corporation\DayBar\Settings.xml";
		public static Settings Load()
		{
			if (!Directory.Exists($@"{FileSys.AppDataPath}\Léo Corporation\DayBar\"))
			{
				Directory.CreateDirectory($@"{FileSys.AppDataPath}\Léo Corporation\DayBar\");
			}

			if (!File.Exists(SettingsPath))
			{
				Global.Settings = new();

				// Serialize to XML
				XmlSerializer xmlSerializer = new(typeof(Settings));
				StreamWriter streamWriter = new(SettingsPath);
				xmlSerializer.Serialize(streamWriter, Global.Settings);
				streamWriter.Dispose();
				return new();
			}

			// If there's already a setting file
			// Deserialize from xml
			XmlSerializer xmlDeserializer = new(typeof(Settings));

			StreamReader streamReader = new(SettingsPath);
			var settings = (Settings?)xmlDeserializer.Deserialize(streamReader) ?? new();
			streamReader.Dispose();

			settings.StartMinute ??= 0;
			settings.EndMinute ??= 0;
			settings.NotifyPercentage ??= false;
			settings.NotifyPercentageValue ??= 25;
			settings.NotificationDays ??= new()
			{
				Monday = true,
				Tuesday = true,
				Wednesday = true,
				Thursday = true,
				Friday = true,
				Saturday = true,
				Sunday = true
			};
			return settings;
		}

		public static void Save()
		{
			// Serialize to XML
			XmlSerializer xmlSerializer = new(typeof(Settings));
			StreamWriter streamWriter = new(SettingsPath);
			xmlSerializer.Serialize(streamWriter, Global.Settings);
			streamWriter.Dispose();
		}
	}
}
