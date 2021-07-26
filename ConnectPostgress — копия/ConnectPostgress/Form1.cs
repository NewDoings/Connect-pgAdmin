using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectPostgress
{
    public partial class Form1 : Form
    {
        private string coonstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};DataBase={4};",
                    "localhost", 5432, "postgres",
                    "gigaseta165", "Demo");
        private NpgsqlConnection conn;
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(coonstring);
            Select();
        }
        private void Select()
        {
            try
            {
                conn.Open();
                sql = @"select * from st_select()";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();
                dgvData.DataSource = null;
                dgvData.DataSource = dt;
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dvgData_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                rowIndex = e.RowIndex;
                txtFirstName.Text = dgvData.Rows[e.RowIndex].Cells["firstname"].Value.ToString();
                txtMidName.Text = dgvData.Rows[e.RowIndex].Cells["midname"].Value.ToString();
                txtLastName.Text = dgvData.Rows[e.RowIndex].Cells["lastname"].Value.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            rowIndex = -1;
            txtFirstName.Enabled = txtMidName.Enabled = txtLastName.Enabled = true;
            txtFirstName.Text = txtMidName.Text = txtLastName.Text = null;
            txtFirstName.Select();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (rowIndex < 0)
            {
                MessageBox.Show("Plese choose students to uptata");
                return;
            }
            txtFirstName.Enabled = txtMidName.Enabled = txtLastName.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (rowIndex < 0)
            {
                MessageBox.Show("Plese choose students to delete");
                return;
            }
            try
            {
                conn.Open();
                sql = @"select * from st_delete(:_id)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_id", int.Parse(dgvData.Rows[rowIndex].Cells["id"].Value.ToString()));
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("delete successfully");
                    rowIndex = -1;
                    Select();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("delete filed. Error: "+ ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int result = 0;
            if (rowIndex < 0) // insert
            {
                try
                {
                    conn.Open();
                    sql = @"select * from st_insert(:_firstname, :_midname, :_lastname)";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_firstname", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("_midname", txtMidName.Text);
                    cmd.Parameters.AddWithValue("_lastname", txtLastName.Text);
                    result = (int)cmd.ExecuteScalar();
                    conn.Close();
                    if (result == 1)
                    {
                        MessageBox.Show("Inserted successfully");
                        Select();
                    }
                    else
                    {
                        MessageBox.Show("Inserted fail");
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("insurted filed" + ex.Message);
                }
            }
            else //update
            {
                try
                {
                    conn.Open();
                    sql = @"select * from st_update(:_id,:_firstname,:_midname, :_lastname)";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_id", int.Parse(dgvData.Rows[rowIndex].Cells["id"].Value.ToString()));
                    cmd.Parameters.AddWithValue("_firstname", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("_midname", txtMidName.Text);
                    cmd.Parameters.AddWithValue("_lastname", txtLastName.Text);
                    result = (int)cmd.ExecuteScalar();
                    conn.Close();
                    if (result == 1)
                    {
                        MessageBox.Show("Update successfully");
                        Select();
                    }
                    else
                    {
                        MessageBox.Show("Update fail");
                    }
                    
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("insurted filed" + ex.Message);
                }
            }
            result = 0;
            txtFirstName.Enabled = txtMidName.Enabled = txtLastName.Enabled = false;
            txtFirstName.Text = txtMidName.Text = txtLastName.Text = null;
        }

 
    }
}
