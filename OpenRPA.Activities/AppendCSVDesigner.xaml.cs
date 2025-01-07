using Microsoft.VisualBasic.Activities;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Windows;
using System.Windows.Forms;

namespace OpenRPA.Utilities
{
    public partial class AppendCSVDesigner
    {
        public AppendCSVDesigner()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Select CSV File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Create an InArgument<string> expression using a VisualBasicValue
                ModelItem.Properties["FilePath"].SetValue(new InArgument<string>(new VisualBasicValue<string>("\"" + openFileDialog.FileName + "\"")));
            }
        }              
    }
}