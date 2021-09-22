using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace InventoryLogin
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            AcceptButton = LoginButton;
            backgroundWorker1.DoWork += OnDoWork;
            backgroundWorker1.RunWorkerAsync(1);
        }

        public static bool LoadDatabase(CheckBox Status, Button Login, Button Register, Label label1)
        {
            if (Database.InitMongoClient() == false || Database.PingDatabase() == false)
            {
                LoadDatabase(Status, Login, Register, label1);
            }
            else
            {
                System.Threading.ThreadStart threadParams = new System.Threading.ThreadStart(delegate { LoadDatabaseSafe(Status, Login, Register, label1); });
                System.Threading.Thread thread2 = new System.Threading.Thread(threadParams);
                thread2.Start();
                return true;
            }

            return false;
        }

        void OnDoWork(object sender, DoWorkEventArgs doWorkArgs)
        {
            LoadDatabase(Status, LoginButton, registerButton, label1);
        }

        public static void LoadDatabaseSafe(CheckBox Status, Button Login, Button Register, Label label1)
        {
            if (Status.InvokeRequired || Login.InvokeRequired || Register.InvokeRequired)
            {
                Action updateDBState = delegate { LoadDatabaseSafe(Status, Login, Register, label1); };
                Status.Invoke(updateDBState);
            }
            else
            {
                label1.Visible = false;
                Status.CheckState = CheckState.Checked;
                Login.Enabled = true;
                Register.Enabled = true;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Authentication.Login(userText.Text, passText.Text);

            if (Authentication.loggedIn)
            {
                Hide();
                Portal inventory = new Portal();
                inventory.ShowDialog();
                Close();
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            Authentication.Register(userText.Text, passText.Text);
        }
    }
}
