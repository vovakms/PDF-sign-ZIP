using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfSigZip
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            textBox1.Text = Properties.Settings.Default.outstr;
            textBox2.Text = Properties.Settings.Default.log;
            textBox3.Text = Properties.Settings.Default.acp;
        }

        private void button1_Click(object sender, EventArgs e) // кн. выбор папки для бэкапов ZIP
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;    
            }
        }

        private void button2_Click(object sender, EventArgs e) // кн. выбор журнала
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e) // кн. "Сохранить настройки"
        {
            Properties.Settings.Default.outstr = textBox1.Text;
            Properties.Settings.Default.log = textBox2.Text;
            Properties.Settings.Default.acp = textBox3.Text;
            Properties.Settings.Default.Save();
        }
    }
}
