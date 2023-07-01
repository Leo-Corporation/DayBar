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
using PeyrSharp.Env;
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
		/// <see langword="true"/> if the app should display a banner when an update is available.
		/// </summary>
		public bool NotifyUpdate { get; set; }

		/// <summary>
		/// <see langword="true"/> if the app should display a banner when half of the day has been reached.
		/// </summary>
		public bool NotifyHalfDay { get; set; }

		public Settings()
		{
			// Default configuration
			StartHour = 0;
			EndHour = 24;
			NotifyUpdate = true;
			NotifyHalfDay = false;
			LaunchOnStart = true;
			UseDarkThemeSystemTray = false;
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
