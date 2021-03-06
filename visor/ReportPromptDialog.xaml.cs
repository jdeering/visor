﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Visor.Lib;

namespace Visor.ReportRunner
{
    /// <summary>
    ///     Interaction logic for ReportPromptDialog.xaml
    /// </summary>
    public partial class ReportPromptDialog
    {
        public ReportPromptDialog(IReadOnlyList<ReportPrompt> prompts)
        {
            InitializeComponent();

            for (int i = 0; i < prompts.Count; i++)
            {
                AddPrompt(prompts[i].Type, prompts[i].Text, i);
            }

            var cancel = new Button
                {
                    Content = "Cancel",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 75,
                    Margin = new Thickness(20, 10, 0, 10)
                };
            cancel.Click += Cancel;

            var submit = new Button
                {
                    Content = "Continue",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 75,
                    Margin = new Thickness(0, 10, 20, 10)
                };
            submit.Click += Submit;

            Grid.SetColumn(cancel, 0);
            Grid.SetRow(cancel, prompts.Count);
            Grid.SetColumn(submit, 1);
            Grid.SetRow(submit, prompts.Count);

            Prompts.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
            Prompts.Children.Add(cancel);
            Prompts.Children.Add(submit);
        }

        public List<string> Answers
        {
            get
            {
                var result = new List<string>();

                foreach (UIElement field in Prompts.Children)
                {
                    var picker = field as DatePicker;
                    if (picker != null)
                        result.Add(picker.Text);
                    else
                    {
                        var box = field as TextBox;
                        if (box != null)
                            result.Add(box.Text);
                    }
                }
                return result;
            }
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddPrompt(PromptType type, string prompt, int row)
        {
            var label = new Label
                {
                    Content = prompt,
                    Margin = new Thickness(10, 5, 0, 5)
                };
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, row);

            UIElement field;
            switch (type)
            {
                case PromptType.Date:
                    field = new DatePicker();
                    ((DatePicker) field).Margin = new Thickness(10, 5, 0, 5);
                    ((DatePicker) field).HorizontalAlignment = HorizontalAlignment.Left;
                    ((DatePicker) field).VerticalAlignment = VerticalAlignment.Center;
                    ((DatePicker) field).BorderThickness = new Thickness(0);
                    break;
                default:
                    field = new TextBox();
                    ((TextBox) field).Margin = new Thickness(10, 5, 0, 5);
                    ((TextBox) field).HorizontalAlignment = HorizontalAlignment.Left;
                    ((TextBox) field).VerticalAlignment = VerticalAlignment.Center;
                    break;
            }


            Grid.SetColumn(field, 1);
            Grid.SetRow(field, row);

            Prompts.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
            Prompts.Children.Add(label);
            Prompts.Children.Add(field);
        }
    }
}