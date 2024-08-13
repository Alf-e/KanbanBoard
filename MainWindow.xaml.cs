
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using Timer = System.Timers.Timer;
using SQLitePCL;
using Microsoft.Data.Sqlite;


namespace Kanban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<KanbanItem> readyItems;
        public ObservableCollection<KanbanItem> doingItems;
        public ObservableCollection<KanbanItem> doneItems;
        public ObservableCollection<KanbanItem> sourceCollection;
        
        
        public MainWindow()
        {
            
            InitializeComponent();
            SQLiteHelper.InitializeDatabase();

            PopulateLists();

            readyColumn.InternalItemsControl.ItemsSource = readyItems;
            readyColumn.KanbanListDrop += KanbanList_Drop;
            readyColumn.KanbanItemMouseDown += KanbanItem_MouseDown;

            doingColumn.InternalItemsControl.ItemsSource = doingItems;
            doingColumn.KanbanListDrop += KanbanList_Drop;
            doingColumn.KanbanItemMouseDown += KanbanItem_MouseDown;

            doneColumn.InternalItemsControl.ItemsSource = doneItems;
            doneColumn.KanbanListDrop += KanbanList_Drop;
            doneColumn.KanbanItemMouseDown += KanbanItem_MouseDown;

            
            
        }
        
        private void PopulateLists()
        {
            //readyItems = SQLiteHelper.GetAllKanbanItems();
            readyItems = FillListFromFile("Ready");
            doingItems = FillListFromFile("Doing");
            doneItems = FillListFromFile("Done");
        }

        private ObservableCollection<KanbanItem> FillListFromFile(string type)
        {
            ObservableCollection<KanbanItem> localList = new();

            for (int i = 0; i < 2; i++)
            {
                localList.Add(new KanbanItem());
            }
            

            return localList;
        }

        private void KanbanItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            var border = sender as Border;
            var data = border.DataContext as KanbanItem;
            var selectedHostControl = FindVisualParent<ItemsControl>(border);

            if (data != null)
            {
                
          
                border.Opacity = 0.5;
                this.sourceCollection = selectedHostControl.ItemsSource as ObservableCollection<KanbanItem>;
                DragDrop.DoDragDrop(border, data, DragDropEffects.Move);
                
                
            }
        }

        private void KanbanList_Drop(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(typeof(KanbanItem)))
            {
                var droppedData = e.Data.GetData(typeof(KanbanItem)) as KanbanItem;
               
               
                sourceCollection.Remove(droppedData);

                var targetControl = sender as ItemsControl;
                var targetControlList = targetControl.ItemsSource as ObservableCollection<KanbanItem>;
                targetControlList.Add(droppedData);

                
            }
            
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

       

        public class KanbanItem : INotifyPropertyChanged
        {
            public string Title { get; set; }
            public string Colour { get; set; }
            public string Tag { get; set; }
            public List<SubTask> SubTasks { get; set; }

            private string subTaskTotalString;
            public string SubTaskTotalString
            {
                get { return subTaskTotalString; }
                set
                {
                    if (value != subTaskTotalString)
                    {
                        subTaskTotalString = value;
                        OnPropertyChanged(nameof(SubTaskTotalString));
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;



            public KanbanItem() : this("Test Item", "LightGray", "TestTag")
            {


                //this.Title = "Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item Test Item jfghdhem Test Itemvtsdysytsysssssssssssssssssssssssggggggggggggggggggggggggggg";
                AddSubTask();
                AddSubTask();
                AddSubTask();
                AddSubTask();
                AddSubTask();
                AddSubTask();
            }
            public KanbanItem(string title, string color, string tag)
            {
                this.Title = title;
                this.Colour = color;
                this.Tag = tag;
                this.SubTasks = new();

            }

            private void AddSubTask()
            {

                this.SubTasks.Add(new SubTask(this));
                UpdateRunningTotalString();
            }
            public void UpdateRunningTotalString()
            {
                int runningTotal = 0;
                foreach (var item in SubTasks)
                {
                    if (item.Completed)
                    {
                        runningTotal++;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.SubTaskTotalString = $"{runningTotal}/{SubTasks.Count}";
                });
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


            public class SubTask
            {
                public string Title { get; set; }
                private bool completed;
                private KanbanItem host;
                public bool Completed
                {
                    get { return this.completed; }
                    set
                    {
                        this.completed = value;
                        host.UpdateRunningTotalString();
                    }
                }

                public SubTask()
                {
                }

                public SubTask(KanbanItem host) : this("Test Subtask", false, host)
                {

                }

                public SubTask(string title, bool completed, KanbanItem host)
                {
                    this.host = host;
                    Title = title;
                    Completed = completed;

                }
            }
        }

        public static class SQLiteHelper
        {
            private const string connectionString = "Data Source=my_database.db";

            public static void InitializeDatabase()
            {
                SQLitePCL.Batteries_V2.Init();
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
    CREATE TABLE IF NOT EXISTS Kitems (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Title TEXT NOT NULL,
        Colour TEXT NOT NULL,
        Tag TEXT NOT NULL
    )";

                    using (var command = new SqliteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }



                    connection.Close();
                }
                   // creates database if doesnt exist
                }

            public static void InsertKanbanItem(KanbanItem item)
            {
                //using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                //{
                //    connection.Open();

                //    using (SQLiteCommand command = new SQLiteCommand(
                //        "INSERT INTO KItems (Title, Colour, Tag) VALUES (@Title, @Colour, @Tag)",
                //        connection))
                //    {
                //        command.Parameters.AddWithValue("@Title", item.Title);
                //        command.Parameters.AddWithValue("@Colour", item.Colour);
                //        command.Parameters.AddWithValue("@Tag", item.Tag);
                //        command.ExecuteNonQuery();
                //    }
                //}
            }

            public static ObservableCollection<KanbanItem> GetAllKanbanItems()
            {
                ObservableCollection<KanbanItem> Kitems = new();

                //using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                //{
                //    connection.Open();

                //    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM KItems", connection))
                //    {
                //        using (SQLiteDataReader reader = command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                KanbanItem item = new KanbanItem
                //                {
                //                    Title = Convert.ToString(reader["Title"]),
                //                    Colour = Convert.ToString(reader["Colour"]),
                //                    Tag = Convert.ToString(reader["Tag"])
                //                };

                //                Kitems.Add(item);
                //            }
                //        }
                //    }
                //}

                return Kitems;
            }
        }
    }
}
