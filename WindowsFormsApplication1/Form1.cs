using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);


        public Form1()
        {
            InitializeComponent();
        }

        string m_Remote_Map_Drive = System.Configuration.ConfigurationManager.AppSettings["Remote_Map_Drive"].ToString();
        clsNetworkDrive m_oNetDrive = new clsNetworkDrive();

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string lcl_Remote_User_Folder = System.Configuration.ConfigurationManager.AppSettings["Remote_User_Folder"].ToString();
            string lcl_Remote_User_Id = System.Configuration.ConfigurationManager.AppSettings["Remote_UserId"].ToString();
            string lcl_Remote_User_Pwd = System.Configuration.ConfigurationManager.AppSettings["Remote_UserPwd"].ToString();
            this.IsMapDriveCreated(m_Remote_Map_Drive, lcl_Remote_User_Folder, lcl_Remote_User_Id, lcl_Remote_User_Pwd);
        }

        private void IsMapDriveCreated(string sMapDriveLetter, string sSharePath,string sUserId, string sUserPwd)
        {
            try
            {
                if (!(Directory.Exists(sMapDriveLetter + ":")))
                {
                    
                    m_oNetDrive.Force = true;
                    m_oNetDrive.Persistent = true;
                    m_oNetDrive.LocalDrive = sMapDriveLetter;
                    m_oNetDrive.PromptForCredentials = false;
                    m_oNetDrive.ShareName = sSharePath;
                    m_oNetDrive.SaveCredentials = true;
                    if (rdoYes.Checked)
                    {
                        m_oNetDrive.MapDrive(sUserId, sUserPwd);
                    }
                    else
                    {
                        m_oNetDrive.MapDrive();
                    }
                    MessageBox.Show("Done", "Message");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(m_Remote_Map_Drive + ":"))
            {
                m_oNetDrive.UnMapDrive();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //string lcl_remotepath = System.Configuration.ConfigurationManager.AppSettings["Remote_User_Folder"].ToString();
                //string lcl_remotepath = "\\\\10.10.5.3\\Recordings\\Primary\\Agent\\UNKNOWN\\2016\\Screenconnect\\July2016";
                foreach (string lcl_str in Directory.GetFiles(textBox1.Text, "96e81227-2f53-4e37-b87f-3e1a8bb165ef*"))
                {
                    MessageBox.Show(lcl_str);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IntPtr admin_token = default(IntPtr);
            //Added these 3 lines
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            string lcl_remotepath = System.Configuration.ConfigurationManager.AppSettings["Remote_User_Folder"].ToString();
            //string lcl_remotepath = "\\\\10.10.5.3\\Recordings\\Primary\\Agent\\UNKNOWN\\2016\\Screenconnect\\July2016";

            if (LogonUser("sjangid", "lesterinc.com", "satima_20", 9, 0, ref admin_token) != 0)
            {
                //Newly added lines
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                DirectoryInfo dir = new DirectoryInfo(lcl_remotepath);
                DirectoryInfo[] dirs = dir.GetDirectories();
                MessageBox.Show(dirs.Count().ToString());
            }
        }
    }
}
