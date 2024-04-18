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
	}

	internal void InitUI()
	{
		try
		{
			DueDatePicker.SelectedDate = DateTime.Now;

			TodayTasksPanel.Children.Clear();
			OtherTasksPanel.Children.Clear();

			List<TodoTask> today = new();
			List<TodoTask> other = new();

			for (int i = 0; i < Global.Todos[0].Tasks.Count; i++)
			{
				if (Global.Todos[0].Tasks[i].DueDate.Year == DateTime.Now.Year && Global.Todos[0].Tasks[i].DueDate.Month == DateTime.Now.Month && Global.Todos[0].Tasks[i].DueDate.Day == DateTime.Now.Day)
				{
					today.Add(Global.Todos[0].Tasks[i]);
				}
				else
				{
					other.Add(Global.Todos[0].Tasks[i]);
				}
			}

			for (int i = 0; i < today.Count; i++)
			{
				TodayTasksPanel.Children.Add(new ToDoItem(today[i], TodayTasksPanel));
			}
			for (int i = 0; i < other.Count; i++)
			{
				OtherTasksPanel.Children.Add(new ToDoItem(other[i], OtherTasksPanel));
			}
		}
		catch { }
	}

	private void AddBtn_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(TaskTitleTxt.Text)) return;
		Global.Todos[0].Tasks.Add(new(DueDatePicker.SelectedDate ?? DateTime.Now, TaskTitleTxt.Text, false));
		TodoManager.Save();
		InitUI();
	}
}
