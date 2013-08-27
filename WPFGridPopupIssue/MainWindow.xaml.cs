using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFGridPopupIssue
{

    public class TestDataValue
    {
        public String Name { get; set; }
        public double Value { get; set; }
    }

    public class TestDataCollection : ObservableCollection<TestDataValue>
    {
        public TestDataCollection()
            : base()
        {
            Add(new TestDataValue { Name = "test1", Value = 10.0 });
            Add(new TestDataValue { Name = "test2", Value = 20.0 });
            Add(new TestDataValue { Name = "test3", Value = 30.0 });
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {

                InitializeComponent();

                TestData = new List<TestDataValue>();
                TestData.Add(new TestDataValue { Name = "test1", Value = 10.0 });
                TestData.Add(new TestDataValue { Name = "test2", Value = 20.0 });
                TestData.Add(new TestDataValue { Name = "test3", Value = 30.0 });
            }
            catch (Exception e)
            {
                int a = 0;
                a++;
                throw;
            }
        }

        public List<TestDataValue> TestData { get; set; }
    }


    public class MyDataGrid : DataGrid
    {
        public MyDataGrid()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnButtonClick), true);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button filterButton = e.OriginalSource as Button;
            // Get the header of the textbox
            DataGridColumnHeader header = TryFindParent<DataGridColumnHeader>(filterButton);
            if (header != null)
            {
                string columnBinding = GetColumnBinding(header);

                Popup popupWindow = new Popup();
                

                //popupWindow.LostFocus += delegate { popupWindow.IsOpen = false; }; // ?
                popupWindow.StaysOpen = false;
                popupWindow.Width = 250;
                popupWindow.Height = 250;

                popupWindow.PlacementTarget = filterButton;
                popupWindow.Placement = PlacementMode.Bottom;
                popupWindow.IsOpen = true;

            }
        }

        private string GetColumnBinding(DataGridColumnHeader header)
        {
            // Try to get the property bound to the column.
            // This should be stored as datacontext.
            string columnBinding = header.DataContext != null ? header.DataContext.ToString() : "";
            if (header.Column is DataGridTextColumn)
                columnBinding = ((Binding)((DataGridTextColumn)header.Column).Binding).Path.Path;

            return columnBinding;
        }


        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect
        /// child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found,
        /// a null reference is being returned.</returns>
        public static T TryFindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);
            //we've reached the end of the tree
            if (parentObject == null) return null;
            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Do note, that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;
                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }
            // If it's not a ContentElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

    }
}
