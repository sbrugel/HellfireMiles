using System;
using System.IO;
using System.Windows.Forms;

namespace HellfireMiles
{
    public partial class DataLocation : Form
    {
        string movesdir;
        public DataLocation()
        {
            button2.Enabled = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                button2.Enabled = true; //unlock button if a directory is chosen
                movesdir = @folderBrowserDialog1.SelectedPath;
                textBox1.Text = movesdir;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles";
            Directory.CreateDirectory(dir);
            string path = dir + "\\dir.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(textBox1.Text);
                }
            }
            MessageBox.Show("Setup completed, please re-run the program to access all program features.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
