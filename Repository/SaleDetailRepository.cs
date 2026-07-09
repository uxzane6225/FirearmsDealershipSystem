using FirearmsDealershipSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirearmsDealershipSystem.Repository
{
    internal class SaleDetailRepository
    {
        private readonly string _connectionString;
        public SaleDetailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Add(SaleDetail saleDetail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    //string query = "INSERT INTO sales_detail (pid,sid,quantity) VALUES (@pid,@sid,@quantity)";
                    using (var cmd = new MySqlCommand("salesDetail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        /*cmd.Parameters.AddWithValue("@pid", saleDetail.ProductID);
                        cmd.Parameters.AddWithValue("@sid", saleDetail.SaleID);
                        cmd.Parameters.AddWithValue("@quantity", saleDetail.Quantity);*/
                        cmd.Parameters.AddWithValue("@productID", saleDetail.ProductID);
                        cmd.Parameters.AddWithValue("@saleID", saleDetail.SaleID);
                        cmd.Parameters.AddWithValue("@productQuantity", saleDetail.Quantity);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Add Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool Edit(SaleDetail saleDetail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    //string query = "UPDATE sales_detail SET pid = @pid, sid = @sid, quantity = @quantity WHERE sdid = @sdid";
                    using (var cmd = new MySqlCommand("editSalesDetail", conn))
                    {
                        /*cmd.Parameters.AddWithValue("@sdid", saleDetail.SaleDetailID);
                        cmd.Parameters.AddWithValue("@pid", saleDetail.SaleID);
                        cmd.Parameters.AddWithValue("@sid", saleDetail.CID);
                        cmd.Parameters.AddWithValue("quantity", saleDetail.TotalAmount);*/
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@salesDetailID", saleDetail.SaleDetailID);
                        cmd.Parameters.AddWithValue("@saleID", saleDetail.SaleID);
                        cmd.Parameters.AddWithValue("@productID", saleDetail.ProductID);
                        cmd.Parameters.AddWithValue("productQuantity", saleDetail.Quantity);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool Delete(SaleDetail saleDetail)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    //string query = "DELETE FROM sales_detail WHERE sdid = @sdid";
                    using (var cmd = new MySqlCommand("deleteSaleDetail", conn))
                    {
                        //cmd.Parameters.AddWithValue("@sdid", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@salesDetailID", saleDetail.SaleDetailID);
                        cmd.Parameters.AddWithValue("@productID", saleDetail.ProductID);
                        cmd.Parameters.AddWithValue("@salesID", saleDetail.SaleID);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
