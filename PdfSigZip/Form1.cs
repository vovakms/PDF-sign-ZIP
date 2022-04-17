using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Compression;


 
using System.Threading;
 


namespace PdfSigZip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            checksetting(); // проверяем настройки

            string strdir = Properties.Settings.Default.path ; // путь к выбранной папке
            textBox1.Text = strdir;                            // показываем в строке адреса
            if (strdir == "" || !Directory.Exists(strdir))    // проверяем 
            {
                strdir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }

            navmove(strdir);
        }
        
        private void button1_Click(object sender, EventArgs e) // нажали кн. "Выбрать папку"
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                navmove(folderBrowserDialog1.SelectedPath);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (  Directory.Exists(e.Node.Name))
            {
                navmove(e.Node.Name);
            }
        }

        private void button5_Click(object sender, EventArgs e) // нажали кн. "Вверх"
        {
            string path = Directory.GetParent(textBox1.Text).ToString();
            navmove(path);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.path = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void button3_Click(object sender, EventArgs e) // кн. "Подписать"
        {
            sigpdf();
        }

        private void button4_Click(object sender, EventArgs e) // кн. "ZIP"
        {
            wrapzip();
        }

        private void button2_Click(object sender, EventArgs e)  // кн. "Подписать и ZIP"
        {
            sigpdf();
            wrapzip();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e) // меню Настройки
        {
            Form fr = new Form2();
            fr.Show();
        }

        private void checksetting() // проверка настроек
        {
            string strout = Properties.Settings.Default.outstr;
            string log = Properties.Settings.Default.log;
            string pathdesktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            if (strout == "" || !Directory.Exists(strout))
            {
                string pathbackup = pathdesktop + @"\pdfbackup";
                DirectoryInfo dirInfo = new DirectoryInfo(pathbackup);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                Properties.Settings.Default.outstr = pathbackup;
            }

            if (log == "" || File.Exists(log))
            {
                string pathlog = pathdesktop + @"\pdfbackup\!_log.log";

                File.Create(pathlog);
                Properties.Settings.Default.log = pathlog;
            }

            Properties.Settings.Default.Save();
        }


        private void navmove(string strdir)  // 
        {
            int n = 0;                // счетчик pdf-файлов  
            textBox1.Text = strdir;   // показываем
            treeView1.Nodes.Clear();  // очищаем 

            DirectoryInfo dir = new DirectoryInfo(strdir);        // показываем папки
            foreach (DirectoryInfo files in dir.GetDirectories())
            {
                treeView1.Nodes.Add(files.FullName, files.Name, 1, 2);
            }
            foreach (FileInfo files in dir.GetFiles("*.pdf"))     // показываем файлы
            {
                treeView1.Nodes.Add(files.FullName, files.Name, 4, 4);
                n++;
            }
            toolStripStatusLabel2.Text = n.ToString() == "0" ? "PDF-файлы отсутствуют" : n.ToString(); // в строке состояния показываем кол-во pdf-файлов
        }

        private void sigpdf()  // функция подписания всех PDF файлов в указанной папке
        {
            string acp = Properties.Settings.Default.acp;

            string strAdr = textBox1.Text + @"\"; // путь к папке с pdf-файлами

            string batPath = strAdr + "sig.cmd";  // путь к батнику 

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string[] commands = { "chcp 1251", "set CurPath=%~dp0", @"for %%I in (%CurPath%\*.pdf) do call ""C:\Program Files\Crypto Pro\CSP\csptest.exe"" -sfsign -sign -in %%I -out %%I.sig -my """ + acp + @""" -addsigtime -add -detached" };
            using (StreamWriter writer = new StreamWriter(new FileStream(batPath, FileMode.OpenOrCreate), Encoding.GetEncoding("Windows-1251")))
            {
                foreach (string row in commands)
                {
                    writer.WriteLine(row);
                }
            }

            Process cmd = Process.Start(batPath);
            cmd.WaitForExit();
            File.Delete(batPath); // удаляем временно созданный батник
        }

        private void wrapzip() // функция делает ZIP-архив из указанной папки и ложит в указанную папку, и копирует в текущую 
        {
            string outstr = Properties.Settings.Default.outstr;  // путь к папке бэкапов
            DateTime now = DateTime.Now;                         // текущая датавремя
            string curdate = now.ToString("yyyy-MM-dd_HHmmss");  // 
            string pathback = outstr + @"\" + curdate + @".zip"; // путь zip-файлу  

            // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ZipFile.CreateFromDirectory(textBox1.Text, pathback, 0, false, Encoding.GetEncoding("Windows-1251"));

            toolStripStatusLabel3.Text = " упакованы в ZIP";

            File.Copy(pathback, textBox1.Text + @"\" + curdate + @".zip");

            write2log(curdate + " " + textBox1.Text + " " + pathback + "  " + toolStripStatusLabel2.Text );
        }

        private void write2log(string str)
        {
             string log = Properties.Settings.Default.log;
            //StreamWriter f = new StreamWriter(log, true);
            //f.WriteLine( str );
            //f.Close();

            StreamWriter writer = new StreamWriter(log, true);
            writer.WriteLine(Environment.NewLine + str );
            writer.Close();


            //FileStream fs = new FileStream(log, FileMode.Append);
            //StreamWriter w = new StreamWriter(fs, Encoding.Default);
            //w.WriteLine(str);



        }
    }
}
