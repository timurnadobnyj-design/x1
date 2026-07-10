using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Skilful.Data
{
    public partial class ITinvest_password : Form
    {
        public ITinvest_password()
        {
            InitializeComponent();
            
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
          
            ITinvest.ip = IPTextBox.Text;
            ITinvest.login = LoginTextBox.Text;
            ITinvest.password = PasswordTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
         
        }

        private void ITinvest_password_Shown(object sender, EventArgs e)
        {
            button_OK.Focus();
        }

        

       
     
    }
}
