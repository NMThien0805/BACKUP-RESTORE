using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class frmLogin : Form
    {
        public void initcmb()
        {
            cmbServer.Items.Add("DESKTOP-KFBV6L6\\N17DCCN171");
            cmbServer.Items.Add("DESKTOP-KFBV6L6\\SERVER_1");
            cmbServer.Items.Add("DESKTOP-KFBV6L6\\SERVER_2");
            cmbServer.Items.Add("DESKTOP-KFBV6L6\\SERVER_3");
            cmbServer.Items.Add("DESKTOP-KFBV6L6");
            cmbServer.SelectedIndex = 0;
            Program.servername = cmbServer.SelectedItem.ToString();
        }
        public frmLogin()
        {
            InitializeComponent();
            initcmb();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbLogin.Text.Trim() == "" || tbPass.Text.Trim() == "")
            {
                MessageBox.Show("login name và mật mã không được trống", "", MessageBoxButtons.OK);
                return;
            }
            Program.mlogin = tbLogin.Text; Program.password = tbPass.Text;
            //MessageBox.Show(Program.servername + Program.mlogin + Program.password, "", MessageBoxButtons.OK);
            if (Program.KetNoi() == 0) return;
            //MessageBox.Show(Program.servername + Program.mlogin + Program.password, "", MessageBoxButtons.OK);
            Form frm = this.CheckExists(typeof(frmMain));
            if (frm != null) frm.Activate();
            else
            {
                frmMain f = new frmMain();
                f.Show();
            }
            Program.mServer = cmbServer.SelectedIndex;

            Program.mloginDN = Program.mlogin;
            Program.passwordDN = Program.password;
            //string strLenh = "EXEC SP_THONGTINDANGNHAP '" + Program.mlogin + "'";

            //MessageBox.Show(strLenh, "", MessageBoxButtons.OK);

            //Program.myReader = Program.ExecSqlDataReader(strLenh, Program.connstr);

            //if (Program.myReader == null) return;
            //Program.myReader.Read();


            //Program.username = Program.myReader.GetString(0);     // Lay user name
            //if (Convert.IsDBNull(Program.username))
            //{
            //    MessageBox.Show("Login bạn nhập không có quyền truy cập dữ liệu\n Bạn xem lại username, password", "", MessageBoxButtons.OK);
            //    return;
            //}

            //Program.mHoten = Program.myReader.GetString(1);
            //Program.mGroup = Program.myReader.GetString(2);
            //MessageBox.Show("Giảng viên - Nhóm: " + Program.mHoten.Trim() + " - " + Program.mGroup, "", MessageBoxButtons.OK);
            //Program.myReader.Close();
            Program.conn.Close();
            //Close();
        }

        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }

        private void cmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbServer.SelectedItem != null)
                try
                {
                    Program.servername = cmbServer.SelectedItem.ToString();
                }
                catch (Exception) { }
        }

        private void tbLogin_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
