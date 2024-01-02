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

namespace Kanban
{
    /// <summary>
    /// Interaction logic for KanbanBox.xaml
    /// </summary>
    public partial class KanbanBox : UserControl
    {
        public ItemsControl ColumnItemsControl => InternalItemsControl;
        public event EventHandler<DragEventArgs> KanbanListDrop;
        public event EventHandler<MouseButtonEventArgs> KanbanItemMouseDown;
        public KanbanBox()
        {
            InitializeComponent();
        }
       

       

        private void KanbanList_Drop(object sender, DragEventArgs e)
        {

            KanbanListDrop?.Invoke(sender, e);
           
        }

        private void KanbanItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
             KanbanItemMouseDown?.Invoke(sender, e);
           
        }

    }
}
