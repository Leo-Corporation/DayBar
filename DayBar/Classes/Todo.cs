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
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DayBar.Classes;
public class TodoList
{
	public string Title { get; set; }
	public List<TodoTask> Tasks { get; set; }

	public TodoList()
	{
		Tasks = [];
		Title = string.Empty;
	}

	public TodoList(string title, List<TodoTask> tasks)
	{
		Title = title;
		Tasks = tasks;
	}
}

public class TodoTask
{
	public TodoTask(DateTime dueDate, string title, bool done = false)
	{
		DueDate = dueDate;
		Title = title;
		Done = done;
	}

	public TodoTask()
	{
		DueDate = DateTime.Now;
		Title = string.Empty;
		Done = false;
	}

	public DateTime DueDate { get; set; }
	public string Title { get; set; }
	public bool Done { get; set; }
}

public static class TodoManager
{
	private static string TodoPath => $@"{FileSys.AppDataPath}\Léo Corporation\DayBar\Todos.xml";

	public static List<TodoList> Load()
	{
		if (!Directory.Exists($@"{FileSys.AppDataPath}\Léo Corporation\DayBar\"))
		{
			Directory.CreateDirectory($@"{FileSys.AppDataPath}\Léo Corporation\DayBar\");
		}

		if (!File.Exists(TodoPath))
		{
			Global.Todos = [new(Properties.Resources.ToDoList, [])];

			// Serialize to XML
			XmlSerializer xmlSerializer = new(typeof(List<TodoList>));
			StreamWriter streamWriter = new(TodoPath);
			xmlSerializer.Serialize(streamWriter, Global.Todos);
			streamWriter.Dispose();
			return [];
		}

		// Deserialize from xml
		XmlSerializer xmlDeserializer = new(typeof(List<TodoList>));

		StreamReader streamReader = new(TodoPath);
		var todos = (List<TodoList>?)xmlDeserializer.Deserialize(streamReader) ?? [];
		streamReader.Dispose();

		return todos;
	}

	public static void Save()
	{
		// Serialize to XML
		XmlSerializer xmlSerializer = new(typeof(List<TodoList>));
		StreamWriter streamWriter = new(TodoPath);
		xmlSerializer.Serialize(streamWriter, Global.Todos);
		streamWriter.Dispose();
	}
}