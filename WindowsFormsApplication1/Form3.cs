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
    public partial class Form3 : Form
    {
        delegate void SetTextCallback(string text);
        Thread demoThread;

        public Form3()
        {
            InitializeComponent();
        }

        // This event handler creates a thread that calls a 
        // Windows Forms control in a thread-safe way.
        private void setTextSafeBtn_Click(object sender, EventArgs e)
        {
            this.demoThread =  new Thread(new ThreadStart(this.ThreadProcSafe));
            this.demoThread.Start();
        }

        // This method is executed on the worker thread and makes
        // a thread-safe call on the TextBox control.
        private void ThreadProcSafe()
        {
            this.SetText("This text was set safely.");
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                //Note: passing method name as argument
                SetTextCallback d1 = new SetTextCallback(SetText);
                SetTextCallback d2 = new SetTextCallback(SetText1);
                this.Invoke(d1, new object[] { text });//this means method name
                this.Invoke(d2, new object[] { text });//this means method name
            }
            else
            {
                this.textBox1.Text = text;//this.textbox1 means control name
            }
        }

        private void SetText1(string text)
        {
            this.textBox2.Text = "s";
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.KeyValue.ToString());
        }
    }
}