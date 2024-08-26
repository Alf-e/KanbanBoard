﻿using System;
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
using System.Windows.Shapes;

namespace Kanban
{
    /// <summary>
    /// Interaction logic for ManageSubtaskWindow.xaml
    /// </summary>
    public partial class ManageSubtaskWindow : Window
    {
        public Button SourceBtn;
        public ManageSubtaskWindow(Button sender)
        {
            InitializeComponent();
            SourceBtn = sender;
        }
        public void AddClick(object sender, RoutedEventArgs e)
        {
            MainWindow.KanbanItem temp  = SourceBtn.DataContext as MainWindow.KanbanItem;
            long tempId = MainWindow.SQLiteHelper.InsertSubtaskItem(temp.Id.ToString(), "false", titlebox.Text);
            
            temp.AddSubTask("false", titlebox.Text, tempId);
            this.Close();
        }
        public void AddTabButton(object sender, RoutedEventArgs e)
        {
            DeleteGrid.Visibility = Visibility.Collapsed;
            AddGrid.Visibility = Visibility.Visible;
            AddTabBtn.IsEnabled = false;
            DeleteTabBtn.IsEnabled = true;
        }
        public void DeleteTabButton(object sender, RoutedEventArgs e)
        {
            AddGrid.Visibility = Visibility.Collapsed;
            DeleteGrid.Visibility = Visibility.Visible;
            DeleteTabBtn.IsEnabled = false;
            AddTabBtn.IsEnabled = true;
        }
    }
}
