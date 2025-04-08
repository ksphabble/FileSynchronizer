using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSynchronizer.Forms
{
    public partial class frm_SetGithubToken : Form
    {
        public frm_SetGithubToken()
        {
            InitializeComponent();
        }

        private void frm_SetGithubToken_Load(object sender, EventArgs e)
        {
            txtGithubToken.Text = cls_Global_Settings.GithubToken;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Visible = false;

            if (MessageBox.Show("Github Token为Github设置，不是开发人员请勿触碰，否则造成的问题概不负责，确定？", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cls_Global_Settings.GithubToken = txtGithubToken.Text;
                cls_Global_Settings.SaveInfoToDB();
                label2.Visible = true;
            }
        }
    }
}
