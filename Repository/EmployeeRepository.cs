using FirearmsDealershipSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirearmsDealershipSystem.Repository
{
    internal class EmployeeRepository
    {
        private readonly string _connectionString;
        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Add(Employee employee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);
                    string query = @"INSERT INTO employees (eid, fullname, email, password) VALUES (@eid, @fullname, @email, @password)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@eid", employee.EmployeeID);
                        cmd.Parameters.AddWithValue("@fullname", employee.FullName);
                        cmd.Parameters.AddWithValue("@email", employee.Email);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Add Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Edit(Employee employee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);
                    string query = @"UPDATE employees SET fullname = @fullname, email = @email, password = @password WHERE eid = @eid";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@eid", employee.EmployeeID);
                        cmd.Parameters.AddWithValue("@fullname", employee.FullName);
                        cmd.Parameters.AddWithValue("@email", employee.Email);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"DELETE FROM employees WHERE eid = @eid";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@eid", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
