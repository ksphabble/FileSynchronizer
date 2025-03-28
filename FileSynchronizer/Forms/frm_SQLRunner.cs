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
    public partial class frm_SQLRunner : Form
    {
        public frm_SQLRunner()
        {
            InitializeComponent();
        }

        private void frm_SQLRunner_Load(object sender, EventArgs e)
        {
            PrintAllTables();
        }

        private void PrintAllTables()
        {
            string[] arr_TableNames = cls_Files_InfoDB.GetAllTableName();
            if (arr_TableNames != null)
            {
                txtTableNames.Clear();
                for (int i = 0; i < arr_TableNames.Length; i++)
                {
                    txtTableNames.Text += arr_TableNames[i] + "\n";
                }
            }
        }

        private void btnRunSQL_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtSQLEditor.Text))
            {
                return;
            }

            string str_SQL = (String.IsNullOrEmpty(txtSQLEditor.SelectedText) ? txtSQLEditor.Text : txtSQLEditor.SelectedText);
            string str_SQLOutput = String.Empty;

            if (str_SQL.StartsWith("select ", StringComparison.OrdinalIgnoreCase) && str_SQL.ToLower().Contains(" from "))
            {
                string str_DataLine = String.Empty;
                DataTable dt = cls_Files_InfoDB.SQLEnquiry(str_SQL, out str_SQLOutput);
                if (dt != null)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        str_DataLine += dt.Columns[i].ColumnName + " ";
                    }
                    str_DataLine += "\n";

                    foreach (DataRow dataRow in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            str_DataLine += dataRow[i].ToString() + " ";
                        }
                        str_DataLine += "\n";
                    }
                }
                str_DataLine += str_SQLOutput;
                txtOutput.Text += str_DataLine;
            }
            else if ((str_SQL.StartsWith("update ", StringComparison.OrdinalIgnoreCase) && str_SQL.ToLower().Contains(" set ")) ||
                str_SQL.StartsWith("delete from", StringComparison.OrdinalIgnoreCase))
            {
                int i_Output = cls_Files_InfoDB.SQLExecutor(str_SQL, out str_SQLOutput);
                txtOutput.Text += (i_Output.ToString() + "条记录被执行" + str_SQLOutput + "\n");
            }
            else
            {
                txtOutput.Text += ("非法SQL语句：" + str_SQL);
            }
        }

        private void btnClearResult_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }
    }
}
