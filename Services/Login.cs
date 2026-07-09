using FirearmsDealershipSystem.Forms;
using FirearmsDealershipSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FirearmsDealershipSystem.Services
{
    internal class Login
    {
        private readonly string _connectionString = DBConfig.ConnectionString;

        public Login(string connstring)
        {
            _connectionString = connstring;
        }

        public bool AuthLogin(Employee employee)
        {
            string password = employee.Password;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM employees WHERE email = @email";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", employee.Email);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Invalid email!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                            string storedPassword = reader["password"].ToString();

                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password,storedPassword);

                            if (!isPasswordValid)
                            {
                                MessageBox.Show("Invalid password!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }

                            employee.EmployeeID = reader["eid"].ToString();
                            employee.FullName = reader["fullname"].ToString();

                            Models.Session.CurrentEmployee = employee;

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
