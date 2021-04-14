using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace WindowsFormsApplication2
{
    public partial class frmAction : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmAction()
        {
            InitializeComponent();
        }

        private void frmAction_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            checkboxTime.Checked = true;
            //this.panel1.Controls.Add(this.textBox1);
            //this.panel1.Controls.Add(this.dateEdit1);
            //this.panel1.Controls.Add(this.timeEdit1);
            //this.panel1.Controls.Add(this.label1);
        }
    }
}