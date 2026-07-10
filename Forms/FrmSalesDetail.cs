using FirearmsDealershipSystem.Models;
using FirearmsDealershipSystem.Repository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirearmsDealershipSystem.Forms
{
    public partial class FrmSalesDetail : Form
    {
        string connectionString = DBConfig.ConnectionString;
        public FrmSalesDetail()
        {
            InitializeComponent();
        }

        private void FrmSalesDetail_Load(object sender, EventArgs e)
        {
            LoadSales();
            LoadProducts();
        }

        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtSaleID.Text = dgvSales.SelectedRows[0].Cells["sid"].Value.ToString();
            txtCustomerID.Text = dgvSales.SelectedRows[0].Cells["cid"].Value.ToString();
            txtQuantity.Text = dgvSales.SelectedRows[0].Cells["quantity"].Value.ToString();
            cbProducts.Text = dgvSales.SelectedRows[0].Cells["pid"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtQuantity.Text) || string.IsNullOrEmpty(cbProducts.Text))
            {
                MessageBox.Show("Fill all Sale Detail Fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add Sale Detail?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int quantity = Convert.ToInt32(txtQuantity.Text);

                SaleDetail newSD = new SaleDetail
                {
                    ProductID = Convert.ToInt32(cbProducts.SelectedValue),
                    SaleID = Convert.ToInt32(txtSaleID.Text),
                    Quantity = quantity
                };

                var repo = new SaleDetailRepository(connectionString);
                bool success = repo.Add(newSD);

                if (success)
                {
                    MessageBox.Show("Sale Detail Added Succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtQuantity.Clear();
            dgvSales.ClearSelection();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtQuantity.Text) || string.IsNullOrEmpty(cbProducts.Text))
            {
                MessageBox.Show("Fill all Sale Detail Fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit Sale Detail?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                SaleDetail newSD = new SaleDetail
                {
                    SaleDetailID = Convert.ToInt32(dgvSales.SelectedRows[0].Cells[0].Value.ToString()),
                    ProductID = Convert.ToInt32(cbProducts.SelectedValue),
                    SaleID = Convert.ToInt32(txtSaleID.Text),
                    Quantity = Convert.ToInt32(txtQuantity.Text)
                };

                var repo = new SaleDetailRepository(connectionString);
                bool success = repo.Edit(newSD);

                if (success)
                {
                    MessageBox.Show("Sale Detail Updated Succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtQuantity.Clear();
            dgvSales.ClearSelection();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtQuantity.Text) || string.IsNullOrEmpty(cbProducts.Text))
            {
                MessageBox.Show("Fill all Sale Detail Fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete Sale Detail?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaleDetail deleteSD = new SaleDetail
                {
                    SaleDetailID = Convert.ToInt32(dgvSales.SelectedRows[0].Cells[0].Value.ToString()),
                    ProductID = Convert.ToInt32(cbProducts.SelectedValue),
                    SaleID = Convert.ToInt32(txtSaleID.Text)
                };

                var repo = new SaleDetailRepository(connectionString);
                bool success = repo.Delete(deleteSD);

                if (success)
                {
                    MessageBox.Show("Sale Detail Deleted Succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtQuantity.Clear();
            dgvSales.ClearSelection();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear Category Fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Category Fields Cleared!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSaleID.Clear();
                txtCustomerID.Clear();
                txtQuantity.Clear();
                cbProducts.SelectedIndex = -1;
                LoadSales();
            }
        }

        private void LoadSales()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM sales_detail INNER JOIN sales ON sales_detail.sid = sales.sid";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvSales.DataSource = dt;
                            dgvSales.ClearSelection();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Sales", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT pid, name, price, stock FROM products";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            cbProducts.DataSource = dt;
                            cbProducts.DisplayMember = "name";
                            cbProducts.ValueMember = "pid";
                            cbProducts.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit Sales Detail?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FrmMain mainForm = new FrmMain();
                mainForm.ShowDialog();
                this.Close();
            }
        }
    }
}
