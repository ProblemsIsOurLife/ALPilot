
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALPilot
{
    public partial class configbot : Form
    {
        int selecteditem = 0;
        string file;
        public configbot(string namefile)
        {
            InitializeComponent();

            string[] strings = File.ReadAllLines(namefile);
            listBox1.Items.AddRange(File.ReadAllLines(namefile));
            file = namefile;
        }


        private void ListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = listBox1.SelectedItem.ToString();
            selecteditem = listBox1.SelectedIndex;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(selecteditem);
            listBox1.Items.Insert(selecteditem, textBox1.Text);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
           StreamWriter SaveFile = new StreamWriter(file);
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item);
            }

            SaveFile.Close();

            MessageBox.Show("Programs saved!");

        }
        private void configbot_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           // if (e.KeyValue == 40)// стрелка Вниз //33 pgup,34pgdown
                //MessageBox.Show(e.KeyValue.ToString());
          
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Insert(listBox1.SelectedIndex+1, "\t");
        }

        private void Configbot_KeyUp(object sender, KeyEventArgs e)
        {
            string string1, string2;
            int index;
            e.Handled = true;
            if (e.KeyValue == 33)
            {
                if (listBox1.SelectedIndex == 0)
                {

                }
                else
                {
                    string1 = listBox1.SelectedItem.ToString();
                    index = listBox1.SelectedIndex;
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                    listBox1.Items.Insert(index - 1, string1);
                    listBox1.SelectedIndex = index - 1;
                }
            }
            if (e.KeyValue == 34)
            {
                if (listBox1.SelectedIndex == listBox1.Items.Count-1)
                {

                }
                else
                {
                    string1 = listBox1.SelectedItem.ToString();
                    index = listBox1.SelectedIndex;
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                    listBox1.Items.Insert(index + 1, string1);
                    listBox1.SelectedIndex = index + 1;
                }
            }
        }
    }
}
