
using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.SqlClient;
using System.IO;

namespace IPOKurs
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, bool> _employeeStatus = new Dictionary<string, bool>(); 
        private const string ConnectionString = "Data Source=VLADISLAVPC;Initial Catalog=EmployeeWorkTime;Integrated Security=True;";

        public MainWindow()
        {
            InitializeComponent();
            LoadExistingLog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var employeeWindow = new EmployeeWindow();
            if (employeeWindow.ShowDialog() == true)
            {
                string employeeId = employeeWindow.EmployeeId;
                ProcessEmployeeId(employeeId);
            }
        }

        private void ProcessEmployeeId(string employeeId)
        {
            if (!_employeeStatus.ContainsKey(employeeId))
            {
                _employeeStatus[employeeId] = false;
            }

            if (!_employeeStatus[employeeId])
            {
                LogEntry(employeeId, "Пришел");
                _employeeStatus[employeeId] = true;
            }
            else
            {
                LogEntry(employeeId, "Ушел");
                _employeeStatus[employeeId] = false;
            }
        }

        private void LogEntry(string employeeId, string action)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"INSERT INTO employee_events (timestamp, employee_id, event_type) VALUES (@timestamp, @employee_id, @event_type)", connection))
                {
                    command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    command.Parameters.AddWithValue("@employee_id", employeeId);
                    command.Parameters.AddWithValue("@event_type", action);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void LoadExistingLog()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT timestamp, employee_id, event_type FROM employee_events", connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string employeeId = reader.GetInt32(reader.GetOrdinal("employee_id")).ToString();
                        string eventType = reader.GetString(reader.GetOrdinal("event_type"));


                        if (!_employeeStatus.ContainsKey(employeeId))
                        {
                            _employeeStatus[employeeId] = false;
                        }

                        if (eventType == "Пришел")
                        {
                            _employeeStatus[employeeId] = true;
                        }
                        else if (eventType == "Ушел")
                        {
                            _employeeStatus[employeeId] = false;
                        }
                    }
                }
            }

        }
        private void ViewDataButton_Click(object sender, RoutedEventArgs e)
        {
            var employeeWindow = new EmployeeWindow();
            if (employeeWindow.ShowDialog() == true)
            {
                string employeeId = employeeWindow.EmployeeId;
                ShowEmployeeData(employeeId);
            }
        }

        private void ShowEmployeeData(string employeeId)
        {
            var employeeDataWindow = new EmployeeDataWindow();
            string employeeData = GetEmployeeLogData(employeeId);
            employeeDataWindow.EmployeeDataTextBlock.Text = employeeData;
            employeeDataWindow.ShowDialog();

        }
        private string GetEmployeeLogData(string employeeId)
        {
            string data = "";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT timestamp, event_type FROM employee_events WHERE employee_id = @employee_id ORDER BY timestamp", connection))
                {
                    command.Parameters.AddWithValue("@employee_id", employeeId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) { return "Данные для сотрудника не найдены."; }

                        while (reader.Read())
                        {
                            string timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")).ToString("yyyy-MM-dd HH:mm:ss");
                            string eventType = reader.GetString(reader.GetOrdinal("event_type"));

                            data += $"{timestamp} - {eventType}\n";

                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(data))
                return "Данные для сотрудника не найдены.";

            return data;
        }
    }
}
