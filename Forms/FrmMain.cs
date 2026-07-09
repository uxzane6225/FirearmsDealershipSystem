using FirearmsDealershipSystem.Models;
using FirearmsDealershipSystem.Repository;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirearmsDealershipSystem.Forms
{
    public partial class FrmMain : Form
    {
        string connectionString = DBConfig.ConnectionString;
        public FrmMain()
        {
            InitializeComponent();

            if (Models.Session.isLoggedIn == true)
            {
                lblFullName.Text = Session.CurrentEmployee.FullName.ToString();
                tbControls.Appearance = TabAppearance.FlatButtons;
                tbControls.ItemSize = new Size(0, 1);
                tbControls.SizeMode = TabSizeMode.Fixed;
            }
            
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadGender();
            LoadType();
            LoadCategory();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            tbControls.SelectedTab = tpDashboard;
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            tbControls.SelectedTab = tpInventory;
            LoadInventory();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            tbControls.SelectedTab = tpSales;
            LoadSales();
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            tbControls.SelectedTab = tpEmployees;
            LoadEmployees();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            tbControls.SelectedTab = tpCustomers;
            LoadCustomers();
        }

        //Inventory 
        private void btnAddInventory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProduct.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(cbType.Text) || string.IsNullOrEmpty(cbCategory.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Fill all employee fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add product?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Product newProduct = new Product
                {
                    ProductName = txtProduct.Text,
                    Price = Convert.ToDouble(txtPrice.Text),
                    Stock = int.Parse(txtStock.Text)
                };

                ProductType type = new ProductType
                {
                    TypeID = Convert.ToInt32(cbType.SelectedValue),
                    TypeName = cbType.Text
                };

                Category category = new Category
                {
                    CategoryID = Convert.ToInt32(cbCategory.SelectedValue),
                    CategoryName = cbCategory.Text
                };

                Employee employee = new Employee
                {
                    EmployeeID = Session.CurrentEmployee.EmployeeID.ToString()
                };

                var repo = new ProductRepository(connectionString);
                bool success = repo.Add(newProduct, type, category, employee);

                if (success)
                {
                    MessageBox.Show("Product added succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory();
                }
            }

            txtProduct.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            cbCategory.SelectedIndex = -1;
            cbType.SelectedIndex = -1;
            dgvInventory.ClearSelection();
        }

        private void btnEditInventory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProduct.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(cbType.Text) || string.IsNullOrEmpty(cbCategory.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Fill all employee fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit product?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Product editProduct = new Product
                {
                    ProductID = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells["pid"].Value.ToString()),
                    ProductName = txtProduct.Text,
                    Price = Convert.ToDouble(txtPrice.Text),
                    Stock = Convert.ToInt32(txtStock.Text),
                };

                ProductType editType = new ProductType
                {
                    TypeID = Convert.ToInt32(cbType.SelectedValue),
                    TypeName = cbType.Text,
                };

                Category editCategory = new Category
                {
                    CategoryID = Convert.ToInt32(cbCategory.SelectedValue),
                    CategoryName = cbCategory.Text,
                };

                var repo = new ProductRepository(connectionString);
                bool success = repo.Edit(editProduct, editType, editCategory);

                if (success)
                {
                    MessageBox.Show("Product edited succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory();
                }
            }

            txtProduct.Clear();
            txtPrice.Clear();
            cbCategory.SelectedIndex = -1;
            cbType.SelectedIndex = -1;
            dgvInventory.ClearSelection();
        }

        private void btnDeleteInventory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProduct.Text) || string.IsNullOrEmpty(txtPrice.Text) || string.IsNullOrEmpty(cbType.Text) || string.IsNullOrEmpty(cbCategory.Text))
            {
                MessageBox.Show("Fill all product fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete product?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells["pid"].Value.ToString());

                var repo = new ProductRepository(connectionString);
                bool success = repo.Delete(id);

                if (success)
                {
                    MessageBox.Show("Product edited succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory();
                }
            }

            txtProduct.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            cbCategory.SelectedIndex = -1;
            cbType.SelectedIndex = -1;
            dgvInventory.ClearSelection();
        }

        private void btnClearInventory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear product fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Product fields cleared!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtProduct.Clear();
                txtPrice.Clear();
                txtStock.Clear();
                cbCategory.SelectedIndex = -1;
                cbType.SelectedIndex = -1;
                LoadInventory();
                dgvInventory.ClearSelection();
            }
        }

        private void LoadType()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ptid, prodtype FROM product_type";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            cbType.DataSource = dt;
                            cbType.DisplayMember = "prodtype";
                            cbType.ValueMember = "ptid";
                            cbType.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Product Types", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategory()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ctgryid, category FROM categories";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            cbCategory.DataSource = dt;
                            cbCategory.DisplayMember = "category";
                            cbCategory.ValueMember = "ctgryid";
                            cbCategory.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading product categories", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Open Categories Form?", "Quesiton", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FrmCategories categoryForm = new FrmCategories();
                categoryForm.ShowDialog();
                this.Close();
            }
        }

        private void btnSearchInventory_Click(object sender, EventArgs e)
        {
            SearchInventory(txtSearchInventory.Text);
        }

        private void SearchInventory(string keyword)
        {
            if (string.IsNullOrEmpty(txtSearchLastName.Text))
            {
                MessageBox.Show("Enter a name to find!", "Error");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT p.pid, p.name AS 'Name', pt.ptid, pt.prodtype AS 'Type', ct.ctgryid, ct.category AS 'Category', p.price AS 'Price', p.stock AS 'Stock', e.eid FROM products p INNER JOIN product_type pt ON pt.ptid = p.ptid INNER JOIN categories ct ON ct.ctgryid = p.ctgryid INNER JOIN employees e ON e.eid = p.eid WHERE p.Name LIKE @key";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", '%' + keyword + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvInventory.DataSource = dt;
                            dgvInventory.ClearSelection();
                            dgvInventory.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Searching in Inventory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void dgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInventory.SelectedRows.Count >= 0)
            {
                txtProduct.Text = dgvInventory.SelectedRows[0].Cells["Name"].Value.ToString();
                txtPrice.Text = dgvInventory.SelectedRows[0].Cells["Price"].Value.ToString();
                txtStock.Text = dgvInventory.SelectedRows[0].Cells["Stock"].Value.ToString();
                cbType.Text = dgvInventory.SelectedRows[0].Cells["Type"].Value.ToString();
                cbCategory.Text = dgvInventory.SelectedRows[0].Cells["Category"].Value.ToString();
            }
        }

        private void LoadInventory()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT p.pid, p.name AS 'Name', pt.ptid, pt.prodtype AS 'Type', ct.ctgryid, ct.category AS 'Category', p.price AS 'Price', p.stock AS 'Stock', e.eid FROM products p INNER JOIN product_type pt ON pt.ptid = p.ptid INNER JOIN categories ct ON ct.ctgryid = p.ctgryid INNER JOIN employees e ON e.eid = p.eid ORDER BY pid ASC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvInventory.DataSource = dt;
                            dgvInventory.Columns["pid"].Visible = false;
                            dgvInventory.Columns["ptid"].Visible = false;
                            dgvInventory.Columns["ctgryid"].Visible = false;
                            dgvInventory.Columns["eid"].Visible = false;

                            dgvInventory.ClearSelection();
                            dgvInventory.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Sales
        private void btnAddSale_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtTotal.Text))
            {
                MessageBox.Show("Fill all sale fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add Sale?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                Sale newSale = new Sale
                {
                    CID = int.Parse(txtCustomerID.Text),
                    TotalAmount = double.Parse(txtTotal.Text)
                };

                var repo = new SaleRepository(connectionString);
                bool success = repo.Add(newSale);

                if (success)
                {
                    MessageBox.Show("Sale added succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtTotal.Clear();
            dgvSales.ClearSelection();
        }

        private void btnEditSale_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtTotal.Text))
            {
                MessageBox.Show("Fill all sale fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit Sale?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                Sale editSale = new Sale
                {
                    SaleID = Convert.ToInt32(dgvSales.SelectedRows[0].Cells["cid"].Value.ToString()),
                    CID = int.Parse(txtCustomerID.Text),
                    TotalAmount = double.Parse(txtTotal.Text)
                };

                var repo = new SaleRepository(connectionString);
                bool success = repo.Edit(editSale);

                if (success)
                {
                    MessageBox.Show("Sale edited succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtTotal.Clear();
            dgvSales.ClearSelection();
        }

        private void btnDeleteSale_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCustomerID.Text) || string.IsNullOrEmpty(txtTotal.Text))
            {
                MessageBox.Show("Fill all sale fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete Sale?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                int id = Convert.ToInt32(dgvSales.SelectedRows[0].Cells["cid"].Value.ToString());

                var repo = new SaleRepository(connectionString);
                bool success = repo.Delete(id);

                if (success)
                {
                    MessageBox.Show("Sale deleted succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSales();
                }
            }

            txtCustomerID.Clear();
            txtTotal.Clear();
            dgvSales.ClearSelection();
        }

        private void btnClearSale_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear product fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Product fields cleared!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCustomerID.Clear();
                txtTotal.Clear();
                LoadSales();
            }
        }

        private void btnSalesDetail_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Open Sales Detail Form?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FrmSalesDetail detail = new FrmSalesDetail();
                detail.ShowDialog();
                this.Close();
            }
        }

        private void btnSearchSale_Click(object sender, EventArgs e)
        {
            SearchSale(txtSearchSale.Text);
        }

        private void LoadSales()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT s.sid, c.cid, CONCAT(c.firstname, \" \",c.lastname) AS 'Customer', p.name AS 'Product', pt.prodtype AS 'Type', ct.category AS 'Category', p.price AS 'Price', sd.quantity AS 'Quantity', s.total_amount AS 'Total', s.sale_date AS 'Date' FROM sales_detail sd INNER JOIN products p ON p.pid = sd.pid INNER JOIN sales s ON s.sid = sd.sid INNER JOIN customers c ON c.cid = s.cid INNER JOIN product_type pt ON pt.ptid = p.ptid INNER JOIN categories ct ON ct.ctgryid = p.ctgryid";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvSales.DataSource = dt;
                            dgvSales.Columns["sid"].Visible = false;
                            dgvSales.Columns["cid"].Visible = false;

                            dgvSales.ClearSelection();
                            dgvSales.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Loading Sales", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchSale(string keyword)
        {
            if (string.IsNullOrEmpty(txtSearchSale.Text))
            {
                MessageBox.Show("Enter a sale to find!", "Error");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT s.sid, c.cid, CONCAT(c.firstname, \" \",c.lastname) AS 'Customer', p.name AS 'Product', pt.prodtype AS 'Type', ct.category AS 'Category', p.price AS 'Price', sd.quantity AS 'Quantity', s.total_amount AS 'Total', s.sale_date AS 'Date' FROM sales_detail sd INNER JOIN products p ON p.pid = sd.pid INNER JOIN sales s ON s.sid = sd.sid INNER JOIN customers c ON c.cid = s.cid INNER JOIN product_type pt ON pt.ptid = p.ptid INNER JOIN categories ct ON ct.ctgryid = p.ctgryid WHERE CONCAT(c.firstname, \" \",c.lastname) LIKE @key";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", '%' + keyword + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvSales.DataSource = dt;
                            dgvSales.ClearSelection();
                            dgvSales.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Searching in Sales", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSales.SelectedRows.Count >= 0)
            {
                txtCustomerID.Text = dgvSales.SelectedRows[0].Cells["cid"].Value.ToString();
                txtTotal.Text = dgvSales.SelectedRows[0].Cells["Total"].Value.ToString();
            }
        }

        //Employees
        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEID.Text) || string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Fill all employee fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPassword.Text.Length > 8)
            {
                MessageBox.Show("Password must at least be 8 characters!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add employee?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Employee newEmployee = new Employee
                {
                    EmployeeID = txtEID.Text,
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text
                };

                var repo = new EmployeeRepository(connectionString);
                bool success = repo.Add(newEmployee);

                if (success)
                {
                    MessageBox.Show("Employee added succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                }
            }

            txtEID.Text = "RP000";
            txtFullName.Clear();
            txtEmail.Text = "RP000@RattlePower.com";
            txtPassword.Clear();
            dgvEmployees.ClearSelection();
        }

        private void btnEditEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEID.Text) || string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Fill all employee fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Password must at least be 8 characters!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit employee?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Employee newEmployee = new Employee
                {
                    EmployeeID = txtEID.Text,
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text
                };

                var repo = new EmployeeRepository(connectionString);
                bool success = repo.Edit(newEmployee);

                if (success)
                {
                    MessageBox.Show("Employee edited succesfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                }
            }

            txtEID.Text = "RP000";
            txtFullName.Clear();
            txtEmail.Text = "RP000@RattlePower.com";
            txtPassword.Clear();
            dgvEmployees.ClearSelection();
        }

        private void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count < 1)
            {
                MessageBox.Show("Select an employee!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete employee?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string id = dgvEmployees.SelectedRows[0].Cells[0].Value.ToString();
                var repo = new EmployeeRepository(connectionString);
                if (repo.Delete(id))
                {
                    MessageBox.Show("Employee deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                }
            }

            txtEID.Text = "RP000";
            txtFullName.Clear();
            txtEmail.Text = "RP000@RattlePower.com";
            txtPassword.Clear();
            dgvEmployees.ClearSelection();
        }

        private void btnClearEmployee_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear employee fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                txtEID.Text = "RP000";
                txtFullName.Clear();
                txtEmail.Text = "RP000@RattlePower.com";
                txtSearchEmployee.Clear();
                txtPassword.Clear();
                dgvEmployees.ClearSelection();
                LoadEmployees();
                MessageBox.Show("Employee fields cleared", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar == true)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        public void LoadEmployees()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT eid AS 'Employee ID', fullname AS 'Full name', email AS 'Email', password FROM employees";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvEmployees.DataSource = dt;
                            dgvEmployees.Columns["password"].Visible = false;

                            dgvEmployees.ClearSelection();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database error: Loading Employees", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearchEmployee_Click(object sender, EventArgs e)
        {
            SearchEmployee(txtSearchEmployee.Text);
        }

        private void SearchEmployee(string keyword)
        {
            if (string.IsNullOrEmpty(txtSearchFirstName.Text))
            {
                MessageBox.Show("Enter a name to find!", "Error");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT eid AS 'Employee ID', fullname AS 'Full name', email AS 'Email', password FROM employees WHERE fullname LIKE @key";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", '%' + keyword + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvEmployees.DataSource = dt;
                            dgvEmployees.ClearSelection();
                            dgvEmployees.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding {ex.Message}", "Database Error: Searching an Employee", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count >= 0)
            {
                txtEID.Text = dgvEmployees.SelectedRows[0].Cells["Employee ID"].Value.ToString();
                txtFullName.Text = dgvEmployees.SelectedRows[0].Cells["Full name"].Value.ToString();
                txtEmail.Text = dgvEmployees.SelectedRows[0].Cells["Email"].Value.ToString();
            }
        }

        //Customer
        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(cbGender.Text) || string.IsNullOrEmpty(txtAge.Text) || string.IsNullOrEmpty(txtCustomerEmail.Text) || string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Fill all customer fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Add customer?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Customer newCustomer = new Customer
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Gender = cbGender.Text,
                    Age = int.Parse(txtAge.Text),
                    Email = txtCustomerEmail.Text,
                    Phone = txtPhone.Text
                };

                CustomerRepository repo = new CustomerRepository(connectionString);
                bool success = repo.Add(newCustomer);

                if (success)
                {
                    MessageBox.Show("Customer added successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                }
            }

            txtFirstName.Clear();
            txtLastName.Clear();
            txtAge.Clear();
            txtCustomerEmail.Clear();
            txtPhone.Clear();
            cbGender.SelectedIndex = -1;
            dgvCustomers.ClearSelection();
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(cbGender.Text) || string.IsNullOrEmpty(txtAge.Text) || string.IsNullOrEmpty(txtCustomerEmail.Text) || string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Fill all customer fields!", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Edit customer?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Customer editCustomer = new Customer
                {
                    CID = int.Parse(dgvCustomers.SelectedRows[0].Cells[0].Value.ToString()),
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Gender = cbGender.Text,
                    Age = int.Parse(txtAge.Text),
                    Email = txtCustomerEmail.Text,
                    Phone = txtPhone.Text,
                };

                var repo = new CustomerRepository(connectionString);
                bool success = repo.Edit(editCustomer);

                if (success)
                {
                    MessageBox.Show("Customer edited succesfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                }
            }

            txtFirstName.Clear();
            txtLastName.Clear();
            txtAge.Clear();
            txtCustomerEmail.Clear();
            txtPhone.Clear();
            cbGender.SelectedIndex = -1;
            dgvCustomers.ClearSelection();
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count < 1)
            {
                MessageBox.Show("Select a customer", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Delete customer?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = int.Parse(dgvCustomers.SelectedRows[0].Cells[0].Value.ToString());
                var repo = new CustomerRepository(connectionString);
                if (repo.Delete(id))
                {
                    MessageBox.Show("Employee deleted successfully.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                }
            }

            txtFirstName.Clear();
            txtLastName.Clear();
            txtAge.Clear();
            txtCustomerEmail.Clear();
            txtPhone.Clear();
            cbGender.SelectedIndex = -1;
            dgvCustomers.ClearSelection();
        }

        private void btnClearCustomer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear customer fields?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes)
            {
                txtFirstName.Clear();
                txtLastName.Clear();
                cbGender.SelectedIndex = -1;
                txtAge.Clear();
                txtCustomerEmail.Clear();
                txtPhone.Clear();
                dgvCustomers.ClearSelection();
                LoadCustomers();
                MessageBox.Show("Customer fields cleared", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void LoadCustomers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM customers";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvCustomers.DataSource = dt;
                            ///dgvCustomers.Columns["cid"].Visible = false;

                            dgvCustomers.ClearSelection();
                            dgvCustomers.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database error: Loading Customers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGender()
        {
            cbGender.Items.Add("Male");
            cbGender.Items.Add("Female");
        }

        private void btnSearchFirstName_Click(object sender, EventArgs e)
        {
            SearchFirstName(txtSearchFirstName.Text);
        }

        private void SearchFirstName(string keyword)
        {
            if (string.IsNullOrEmpty(txtSearchFirstName.Text))
            {
                MessageBox.Show("Enter a name to find!", "Error");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM customers WHERE firstname LIKE @key";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", '%' + keyword + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvCustomers.DataSource = dt;
                            dgvCustomers.ClearSelection();
                            dgvCustomers.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Searching First Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearchLastName_Click(object sender, EventArgs e)
        {
            SearchLastName(txtSearchLastName.Text);
        }

        private void SearchLastName(string keyword)
        {
            if (string.IsNullOrEmpty(txtSearchLastName.Text))
            {
                MessageBox.Show("Enter a name to find!", "Error");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM customers WHERE lastname LIKE @key";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", '%' + keyword + '%');
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvCustomers.DataSource = dt;
                            dgvCustomers.ClearSelection();
                            dgvCustomers.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error: Searching Last Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count >= 0)
            {
                txtFirstName.Text = dgvCustomers.SelectedRows[0].Cells["firstname"].Value.ToString();
                txtLastName.Text = dgvCustomers.SelectedRows[0].Cells["lastname"].Value.ToString();
                cbGender.Text = dgvCustomers.SelectedRows[0].Cells["gender"].Value.ToString();
                txtAge.Text = dgvCustomers.SelectedRows[0].Cells["age"].Value.ToString();
                txtCustomerEmail.Text = dgvCustomers.SelectedRows[0].Cells["email"].Value.ToString();
                txtPhone.Text = dgvCustomers.SelectedRows[0].Cells["phone"].Value.ToString();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Logout?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FrmLogin loginForm = new FrmLogin();
                loginForm.ShowDialog();
                this.Close();
            }
        }
    }
}
