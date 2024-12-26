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

namespace IPOKurs
{
    public partial class EmployeeWindow : Window
    {
        public string EmployeeId { get; private set; }

        public EmployeeWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeId = EmployeeIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(EmployeeId))
            {
                MessageBox.Show("Пожалуйста, введите ID сотрудника.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();

        }
    }
}
