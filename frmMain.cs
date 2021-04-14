using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class frmMain : Form
    {
        public int tim_device(string db_name)
        {
            int t = 0;
            string lenh = "SELECT name  FROM sys.backup_devices";
            Program.myReader = Program.ExecSqlDataReader(lenh, Program.connstr);
            if (Program.myReader == null) return t;
            while (Program.myReader.Read())
            {
                if (Program.myReader.GetString(0).Equals("DEV_" + db_name))
                {
                    t = 1;
                    break;
                }
            }
            Program.myReader.Close();
            return t;
        }

        DataTable LoadDATAGripView1()
        {
            var cmd = new SqlCommand($"SELECT [Cơ sở dữ liệu]=name FROM sys.databases where database_id > 4 and name NOT LIKE 'Report%' and name NOT LIKE  'distribution%'", Program.conn);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            da.Dispose();
            return dt;
        }

        DataTable LoadDATAGripView2(string db_name)
        {
            //string s = "SELECT position as [Bản sao lưu thứ #], description as [Diễn giải], " +
            //            " backup_start_date as [Ngày giờ sao lưu], user_name as [User sao lưu] " +
            //    "FROM msdb.dbo.backupset " +
            //    "WHERE database_name = '" + db_name + "' AND type = 'D' AND " +
            //            "backup_set_id >=" +
            //            "(SELECT backup_set_id FROM  msdb.dbo.backupset " +
            //            "WHERE media_set_id = " +
            //                "(SELECT  MAX(media_set_id) " +
            //                "FROM msdb.dbo.backupset " +
            //                "WHERE database_name = '" + db_name + "' AND type = 'D') " +
            //                "AND position = 1) " +
            //                "ORDER BY position DESC";
            //MessageBox.Show(s, "", MessageBoxButtons.OK);
            var cmd = new SqlCommand(
                $"SELECT position as [Bản sao lưu thứ #], description as [Diễn giải], " +
                        " backup_start_date as [Ngày giờ sao lưu], user_name as [User sao lưu] " +
                "FROM msdb.dbo.backupset " +
                "WHERE database_name = '" + db_name + "' AND type = 'D' AND " +
                        "backup_set_id >=" +
                        "(SELECT backup_set_id FROM  msdb.dbo.backupset " +
                        "WHERE media_set_id = " +
                            "(SELECT  MAX(media_set_id) " +
                            "FROM msdb.dbo.backupset " +
                            "WHERE database_name = '" + db_name + "' AND type = 'D') " +
                            "AND position = 1) " +
                            "ORDER BY position DESC"
                , Program.conn);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            da.Dispose();
            return dt;
        }

        public void setSizeColumn2()
        {
            dataGridView2.Columns[0].Width = 150;
            dataGridView2.Columns[1].Width = 225;
            dataGridView2.Columns[2].Width = 225;
            dataGridView2.Columns[3].Width = 150;
        }

        public void setup_button_table2(string db_name)
        {
            if(tim_device(db_name) == 1)
            {
                barButtonItem6.Enabled = false;
                try
                {
                    dataGridView2.DataSource = LoadDATAGripView2(db_name);
                    dataGridView2.CurrentCell = dataGridView2.Rows[0].Cells[0];
                    tbIdbanBK.Caption = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    setSizeColumn2();
                    checkBoxDelAll.Enabled = true;
                    barButtonItem2.Enabled = true;
                    barButtonItem5.Enabled = true;
                    barButtonItem7.Enabled = true;
                    checkBoxDelAll.Enabled = true;
                    barCheckTime.Enabled = true;
                }
                catch (Exception e)
                {
                    dataGridView2.DataSource = null;
                    tbIdbanBK.Caption = "";
                    barButtonItem2.Enabled = false;
                    barButtonItem5.Enabled = false;
                    barButtonItem7.Enabled = false;
                    checkBoxDelAll.Enabled = false;
                    barCheckTime.Enabled = false;
                }
            }
            else
            {
                checkBoxDelAll.Enabled = false;
                barButtonItem6.Enabled = true;
                dataGridView2.DataSource = null;
                tbIdbanBK.Caption = "";
                barButtonItem2.Enabled = false;
                barButtonItem5.Enabled = false;
                barButtonItem7.Enabled = false;
                checkBoxDelAll.Enabled = false;
                barCheckTime.Enabled = false;
            }
        }

        public void inittable()
        {
            dataGridView1.DataSource = LoadDATAGripView1();
            dataGridView1.Columns[0].Width = 160;
            dataGridView1.CurrentCell= dataGridView1.Rows[0].Cells[0];
            setup_button_table2(dataGridView1.CurrentRow.Cells["CƠ SỞ DỮ LIỆU"].Value.ToString());
        }
        public string setCaption1(string s)
        {
            while (s.Length < 10)
            {
                s = " " + s + " ";
                if (s.Length == 9)
                {
                    s = s + " ";
                }
            }
            string s1 = tbTenCSDL.Caption.ToString();
            s1 = s1.Substring(0, s1.Length - 10) + s;
            return s1;
        }

        public string tao_cmd_backup(string db_name)
        {
            string cmd_tao_backup = "BACKUP DATABASE " + db_name + "\n" +
                                    "TO DEV_" + db_name + "\n" +
                                    "WITH DESCRIPTION = N'Bản thứ'," +
                                    "STATS = 10," +
                                    "NAME = '" + db_name + "-Full Database Backup'";
            return cmd_tao_backup;
        }

        public string tao_cmd_del_device(string db_name)
        {
            string cmd_del_device = "EXEC sp_dropdevice 'DEV_" + db_name + "', 'DELFILE' \n" +
                                    "EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = '"+ db_name +"'";
            return cmd_del_device;
        }

        public string tao_cmd_tao_device(string db_name)
        {
            string cmd_tao_device = "EXEC master.dbo.sp_addumpdevice " +
                                    "@devtype = N'disk', " +
                                    "@logicalname = N'DEV_" + db_name + "', " +
                                    "@physicalname = N'F:\\Download\\1HKI\\T3_CD_CNPM\\DEVICE_BACKUP\\DEV_" + db_name + "_" + Program.mServer + ".BAK'";
            return cmd_tao_device;
        }

        public string restore_voi_position(int position, string db_name)
        {
            string cmd_restore = "ALTER DATABASE " + db_name + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                                    "USE tempdb " +
                                    "RESTORE DATABASE " + db_name + " FROM  DEV_" + db_name + " WITH FILE = " + position + ", REPLACE " +
                                    "ALTER DATABASE " + db_name + "  SET MULTI_USER";
            return cmd_restore;
        }

        public string retore_voi_time(string db_name, string ban_gan_nhat, string time)
        {
            string url = "F:/Download/1HKI/T3_CD_CNPM/DEVICE_BACKUP/";
            String strRestore = "ALTER DATABASE " + db_name + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE \n" +
                           " BACKUP LOG " + db_name + " TO DISK ='" + url + "/" + "DEV_" +
                           db_name + ".trn' WITH INIT, NORECOVERY; \n" + " USE tempdb \n " +
                           " RESTORE DATABASE " + db_name + " FROM DEV_" + db_name + " WITH FILE = " + ban_gan_nhat + ",NORECOVERY; \n" +
                           " RESTORE DATABASE " + db_name + " FROM DISK= '" + url + "/" + "DEV_" + db_name + ".trn' " +
                           " WITH STOPAT= '" + time + "' \n" +
                           " ALTER DATABASE  " + db_name + " SET MULTI_USER ";
            MessageBox.Show(strRestore);
            
            return strRestore;
        }

        public int ktra_time(string time, string time_backup)
        {
            int kt = 0;

            DateTime time_input = DateTime.Parse(time);
            DateTime time_ban_cuoi = DateTime.Parse(time_backup);
            DateTime time_now = DateTime.Now;

            if(time_input.Ticks < time_ban_cuoi.Ticks || time_input.Ticks > time_now.Ticks)
            {
                kt = 1;
            }

            //MessageBox.Show(time_input.ToString(), "", MessageBoxButtons.OK);
            //MessageBox.Show(time_ban_cuoi.ToString(), "", MessageBoxButtons.OK);
            //MessageBox.Show(time_now.ToString(), "", MessageBoxButtons.OK);

            return kt;
        }

        public frmMain()
        {
            InitializeComponent();
            inittable();
            String s = dataGridView1.Rows[0].Cells["Cơ sở dữ liệu"].Value.ToString();
            tbTenCSDL.Caption = setCaption1(s);
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //barCheckTime.Checked = true;
            
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void barButtonItem8_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            String s = dataGridView1.CurrentRow.Cells["CƠ SỞ DỮ LIỆU"].Value.ToString();
            
            tbTenCSDL.Caption = setCaption1(s);

            setup_button_table2(s);
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String db_name = dataGridView1.CurrentRow.Cells["CƠ SỞ DỮ LIỆU"].Value.ToString();

            string cmd = "";
            if(checkBoxDelAll.Checked == true)
            {
                cmd = tao_cmd_del_device(db_name) + "\n" + tao_cmd_tao_device(db_name) + "\n" + tao_cmd_backup(db_name);
            }
            else
            {
                if(tim_device(db_name) == 0)
                {
                    cmd = tao_cmd_tao_device(db_name) + "\n" + tao_cmd_backup(db_name);
                }
                else
                {
                    cmd = tao_cmd_backup(db_name);
                }
            }
            //MessageBox.Show(cmd, "", MessageBoxButtons.OK);
            Program.ExecSqlNonQuery(cmd, Program.connstr);

            setup_button_table2(db_name);

            checkBoxDelAll.Checked = false;
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String db_name = dataGridView1.CurrentRow.Cells["CƠ SỞ DỮ LIỆU"].Value.ToString();
            string cmd = tao_cmd_tao_device(db_name);
            //MessageBox.Show(cmd, "", MessageBoxButtons.OK);
            Program.ExecSqlNonQuery(cmd, Program.connstr);
            barButtonItem6.Enabled = false;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(barCheckTime.Checked == true)
            {
                string time_backup = dataGridView2.Rows[0].Cells[2].Value.ToString();
                string time = dateEdit1.DateTime.Month.ToString() + "/" + dateEdit1.DateTime.Day.ToString() + "/" + dateEdit1.DateTime.Year.ToString();
                time =  time + " " + timeEdit1.Time.Hour.ToString() + ":" + timeEdit1.Time.Minute.ToString() + ":" + timeEdit1.Time.Second.ToString();
                if (ktra_time(time, time_backup) == 0)
                {
                    string ban_gan_nhat = dataGridView2.Rows[0].Cells[0].Value.ToString();
                    string db_name = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    //retore_voi_time(db_name, ban_gan_nhat, time);
                    //int kiemtra = ktra_time(time, ban_gan_nhat);
                    //MessageBox.Show(kiemtra.ToString(), "", MessageBoxButtons.OK);
                    int kiemtra = Program.ExecSqlNonQuery(retore_voi_time(db_name, ban_gan_nhat, time)
                                    , Program.connstr);
                    if (kiemtra == 1)
                    {
                        MessageBox.Show("Phục hồi cơ sở dữ liệu thành công.", "", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Phục hồi thất bại!!!", "", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Thời gian không hợp lý!!!", "", MessageBoxButtons.OK);
                }
            }
            else
            {
                int n = Int32.Parse(dataGridView2.CurrentRow.Cells[0].Value.ToString());
                string db_name = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                Program.ExecSqlNonQuery(restore_voi_position(n,db_name), Program.connstr);
                //MessageBox.Show(restore_voi_position(n, db_name), "", MessageBoxButtons.OK);
                MessageBox.Show("Hoàn tất Restore", "", MessageBoxButtons.OK);
            }
        }

        private void barCheckTime_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(barCheckTime.Checked == true)
            {
                this.panel1.Controls.Add(this.textBox1);
                this.panel1.Controls.Add(this.dateEdit1);
                this.panel1.Controls.Add(this.timeEdit1);
                this.panel1.Controls.Add(this.label1);
            }
            else
            {
                this.panel1.Controls.Remove(this.textBox1);
                this.panel1.Controls.Remove(this.dateEdit1);
                this.panel1.Controls.Remove(this.timeEdit1);
                this.panel1.Controls.Remove(this.label1);
            }
        }
    }
}
