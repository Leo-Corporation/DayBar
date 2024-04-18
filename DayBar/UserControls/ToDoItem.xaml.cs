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

namespace DayBar.UserControls;
/// <summary>
/// Interaction logic for ToDoItem.xaml
/// </summary>
public partial class ToDoItem : UserControl
{
	public TodoTask TodoTask { get; }
	public StackPanel ParentPanel { get; }

	public ToDoItem(TodoTask todoTask, StackPanel parentPanel)
	{
		InitializeComponent();
		TodoTask = todoTask;
		ParentPanel = parentPanel;

		InitUI();
	}

	private void InitUI()
	{
		TaskTitleTxt.Text = TodoTask.Title;
		Check.IsChecked = TodoTask.Done;
	}

	private void DeleteBtn_Click(object sender, RoutedEventArgs e)
	{
		ParentPanel.Children.Remove(this);
		Global.Todos[0].Tasks.Remove(TodoTask);
		Global.ToDoPage.InitUI();
		TodoManager.Save();
	}

	private void Check_Checked(object sender, RoutedEventArgs e)
	{
		Global.Todos[0].Tasks[Global.Todos[0].Tasks.IndexOf(TodoTask)].Done = Check.IsChecked ?? false;
		TodoManager.Save();
		if (Global.ToDoPage is null) return;
		Global.ToDoPage.InitProgressUI();
	}
}
