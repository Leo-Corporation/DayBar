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
using DayBar.UserControls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DayBar.Pages;
/// <summary>
/// Interaction logic for ToDoPage.xaml
/// </summary>
public partial class ToDoPage : Page
{
	public ToDoPage()
	{
		InitializeComponent();
		Global.Todos = TodoManager.Load();
		InitUI();
		DueDatePicker.SelectedDate = DateTime.Now;
	}
	List<TodoTask> _today = [];
	List<TodoTask> _other = [];
	internal void InitUI()
	{
		ProgressTxt.Text = string.Format(Properties.Resources.TodoProgressTxt, 0);
		try
		{
			TodayTasksPanel.Children.Clear();
			OtherTasksPanel.Children.Clear();

			_today = [];
			_other = [];

			for (int i = 0; i < Global.Todos[0].Tasks.Count; i++)
			{
				if (Global.Todos[0].Tasks[i].DueDate.Year == DateTime.Now.Year && Global.Todos[0].Tasks[i].DueDate.Month == DateTime.Now.Month && Global.Todos[0].Tasks[i].DueDate.Day == DateTime.Now.Day)
				{
					_today.Add(Global.Todos[0].Tasks[i]);
				}
				else
				{
					_other.Add(Global.Todos[0].Tasks[i]);
				}
			}
			int todayDone = 0;
			for (int i = 0; i < _today.Count; i++)
			{
				TodayTasksPanel.Children.Add(new ToDoItem(_today[i], TodayTasksPanel));
				if (_today[i].Done) todayDone++;
			}
			for (int i = 0; i < _other.Count; i++)
			{
				OtherTasksPanel.Children.Add(new ToDoItem(_other[i], OtherTasksPanel));
			}

			ProgressTxt.Text = string.Format(Properties.Resources.TodoProgressTxt, (int)(todayDone / ((double)_today.Count == 0 ? 1 : (double)_today.Count) * 100d));
			ProgressBar.Value = (int)(todayDone / (double)_today.Count * 100d);
		}
		catch { }
	}

	internal void InitProgressUI()
	{
		int todayDone = 0;
		for (int i = 0; i < _today.Count; i++)
		{
			if (_today[i].Done) todayDone++;
		}

		ProgressTxt.Text = string.Format(Properties.Resources.TodoProgressTxt, (int)(todayDone / ((double)_today.Count == 0 ? 1 : (double)_today.Count) * 100d));
		ProgressBar.Value = (int)(todayDone / (double)_today.Count * 100d);
	}

	private void AddBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(TaskTitleTxt.Text)) return;
		Global.Todos[0].Tasks.Add(new(DueDatePicker.SelectedDate ?? DateTime.Now, TaskTitleTxt.Text, false));
		TodoManager.Save();
		InitUI();
		TaskTitleTxt.Text = string.Empty;
	}

	private void TaskTitleTxt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == System.Windows.Input.Key.Enter)
		{
			AddBtn_Click(sender, e);
		}
	}
}
