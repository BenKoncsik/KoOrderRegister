using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace KoOrderRegister.View
{
    public partial class RenameDialog : Window
    {
        public string NewName { get; private set; }

        public RenameDialog(string currentName)
        {
            InitializeComponent();
            RenameTextBox.Text = currentName;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewName = RenameTextBox.Text;
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NewName = null;
            DialogResult = false;
            this.Close();
        }
    }

}
