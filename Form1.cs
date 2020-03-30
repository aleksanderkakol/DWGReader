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
        public static int sys_id;
        public static int grp_id;
        public static int y;
        public static int x;
        public static int size;

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
                try
                {
                    List<string> cadList = cad.SearchInsertsAttributtesInDWGAutoCadFile(sourceFilePath);
                    foreach (var item in cadList)
                    {
                        checkedListBox1.Items.AddRange(new object[] { item });
                    }
                    label3.Visible = true;
                    checkedListBox1.Visible = true;
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                
            }
        }

        private int stringToInt(string text)
        {
            return Int32.Parse(text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("ID nie może być puste");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Wybierz plik");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Podaj sys ID");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Podaj grp ID");
                return;
            }

            try
            {
                string sourceFilePath = textBox1.Text;
                map_id = stringToInt(textBox2.Text);
                sys_id = stringToInt(textBox3.Text);
                grp_id = stringToInt(textBox4.Text);
                y = stringToInt(textBox5.Text);
                x = stringToInt(textBox6.Text);
                size = stringToInt(textBox7.Text);

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

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

    }
}
