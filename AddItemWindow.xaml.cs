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
using System.Windows.Shapes;

namespace Kanban
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        public static MainWindow.KanbanItem? CreatedItem { get; set; }
        public AddItemWindow()
        {
            InitializeComponent();
            CreatedItem = null;
        }

        public void AddClick(object sender, RoutedEventArgs e)
        {
            MainWindow.KanbanItem temp = new(0, titlebox.Text, colourbox.Text, tagbox.Text);

            temp.Id = MainWindow.SQLiteHelper.InsertKanbanItem(temp);
            CreatedItem = temp;
            this.Close();
        }
        
    }
}
