using FirearmsDealershipSystem.Models;
using FirearmsDealershipSystem.Repository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirearmsDealershipSystem.Forms
{
    public partial class FrmCategories : Form
    {
        string connectionString = DBConfig.ConnectionString;
        int CategoryID;
        public FrmCategories()
        {
            InitializeComponent();
            LoadCategory();
        }

        private void dgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCategory.SelectedRows.Count >= 0)
            {
                CategoryID = int.Parse(dgvCategory.SelectedRows[0].Cells["ctgryid"].Value.ToString());
                txtCategory.Text = dgvCategory.SelectedRows[0].Cells["Category"].Value.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategory.Text))
            {
                MessageBox.Show("Fill the category field!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add category?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Category newCategory = new Category
                {
                    CategoryName = txtCategory.Text
                };

                var repo = new CategoryRepository(connectionString);
                bool success = repo.Add(newCategory);

                if (success)
                {
                    MessageBox.Show("Category added succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCategory();
                }
            }

            txtCategory.Clear();
            dgvCategory.ClearSelection();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategory.Text))
            {
                MessageBox.Show("Fill the category field!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit category?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Category editCategory = new Category
                {
                    CategoryID = int.Parse(dgvCategory.SelectedRows[0].Cells["ctgryid"].Value.ToString()),
                    CategoryName = txtCategory.Text
                };

                var repo = new CategoryRepository(connectionString);
                bool success = repo.Edit(editCategory);

                if (success)
                {
                    MessageBox.Show("Category edited succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCategory();
                }
            }

            txtCategory.Clear();
            dgvCategory.ClearSelection();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategory.Text))
            {
                MessageBox.Show("Fill the category field!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete product?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = int.Parse(dgvCategory.SelectedRows[0].Cells["ctgryid"].Value.ToString());

                var repo = new CategoryRepository(connectionString);
                bool success = repo.Delete(id);

                if (success)
                {
                    MessageBox.Show("Category deleted succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCategory();
                }
            }

            txtCategory.Clear();
            dgvCategory.ClearSelection();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Leave products?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FrmMain mainForm = new FrmMain();
                mainForm.ShowDialog();
                this.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear product fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Product fields cleared!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCategory.Clear();
                dgvCategory.ClearSelection();
                LoadCategory();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search(txtSearch.Text);
        }

        private void LoadCategory()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM categories";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvCategory.DataSource = dt;

                            dgvCategory.ClearSelection();
                            dgvCategory.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Category", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Search(string word)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Enter a category to find!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM categories WHERE category LIKE @word";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@word", '%' + word + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvCategory.DataSource = dt;
                            dgvCategory.ClearSelection();
                            dgvCategory.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Searching Category", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
