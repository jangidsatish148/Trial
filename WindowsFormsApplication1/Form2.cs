using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public class clsData
        {
            public int m_iMax, m_iMin, m_iCount;

            public clsData()
            {
            }

            public clsData(int p_iMin, int p_iMax)
            {
                m_iMax = p_iMax;
                m_iMin = p_iMin;
            }
        }
        
        //You can create object of BackgroundWorker or drag & drop it from toolbox
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        public Form2()
        {
            InitializeComponent();

            btnCancel.Enabled = false;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            //backgroundWorker1.DoWork += delegate(object sender, DoWorkEventArgs e){};
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            
            //Tell the user how the process went
            backgroundWorker1.WorkerReportsProgress = true;

            //Allow for the process to be cancelled
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            if (!backgroundWorker1.IsBusy)
            {
                btnStart.Enabled = false;
                btnCancel.Enabled = true;

                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                txtWorkerThread.Clear();

                clsData lcl_obj = new clsData(progressBar1.Minimum, progressBar1.Maximum);
                backgroundWorker1.RunWorkerAsync(lcl_obj);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                btnStart.Enabled = true;
                btnCancel.Enabled = false;
                backgroundWorker1.CancelAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //No form's control should be used in this method

            clsData lcl_obj = (clsData)e.Argument;
            if (lcl_obj == null) return;

            DataTable lcl_dt = new DataTable("tblData");
            lcl_dt.Columns.Add("Id");
            for (int i = lcl_obj.m_iMin; i <= lcl_obj.m_iMax; i++)
            {
                Thread.Sleep(100);

                lcl_obj.m_iCount = i;
                lcl_dt.Rows.Add(i);
                backgroundWorker1.ReportProgress(i, lcl_obj);

                //Check if there is a request to cancel the process
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    backgroundWorker1.ReportProgress(0);
                    return;
                }
            }
                  
            e.Result = lcl_dt;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Form's control should be used in this method
            progressBar1.Value = e.ProgressPercentage;
            lblPBStatus.Text = e.ProgressPercentage + "%";

            clsData lcl_obj = (clsData)e.UserState;
            txtWorkerThread.AppendText(lcl_obj.m_iMin + " " + lcl_obj.m_iMax + " " + lcl_obj.m_iCount);
            txtWorkerThread.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Form's control should be used in this method
            if (e.Cancelled)
            {
                lblPBStatus.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                lblPBStatus.Text = "There was an error running the process. The thread aborted";
            }
            else
            {
                lblPBStatus.Text = "Process was completed";
            }
            btnStart.Enabled = true;
            btnCancel.Enabled = false;

            DataTable lcl_dt = new DataTable();
            lcl_dt = (DataTable)e.Result;
            dataGridView1.DataSource = lcl_dt;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
