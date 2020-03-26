using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace DWG
{
    public partial class Form1 : Form
    {
        OpenFileDialog myFileDialog = new OpenFileDialog();
        public static int map_id;
        CAD cad = new CAD();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myFileDialog.RestoreDirectory = true;
            myFileDialog.CheckFileExists = true;
            myFileDialog.CheckPathExists = true;
            myFileDialog.Title = "Przeglądaj AutoCad Files";
            myFileDialog.DefaultExt = "dwg";
            myFileDialog.Filter = "AutoCAD Drawing (*.dwg)|*.dwg|All files (*.*)|*.*";

            if (myFileDialog.ShowDialog() == DialogResult.OK)
            {
                checkedListBox1.Items.Clear();
                textBox1.Text = myFileDialog.FileName;
                string sourceFilePath = textBox1.Text;
                
                List<string> cadList = cad.SearchInsertsAttributtesInDWGAutoCadFile(sourceFilePath);
                foreach (var item in cadList)
                {
                    checkedListBox1.Items.AddRange(new object[] { item });
                }
                label3.Visible = true;
                checkedListBox1.Visible = true;
            }
        }

        public int map_ID()
        {
            return Int32.Parse(textBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("ID nie może być puste");
                return;
            }
            map_id = map_ID();


            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Wybierz plik");
                return;
            }
            string sourceFilePath = textBox1.Text;

            List<object> itemsChecked = new List<object>();
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                itemsChecked.Add(itemChecked);
            }

            Thread myThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                cad.SearchTextInDWGAutoCADFile(sourceFilePath, itemsChecked);
            });
            myThread.Start();
        }
    }
}
