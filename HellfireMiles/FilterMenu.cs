using System;
using System.Windows.Forms;

namespace HellfireMiles
{
    public partial class FilterMenu : Form
    {
        public FilterMenu()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            if (comboBox1.SelectedItem.Equals("Week"))
            {
               numericUpDown1.Enabled = true;
            }
            else if (comboBox1.SelectedItem.Equals("Class"))
            {
                textBox1.Enabled = true;
            }
            else if (comboBox1.SelectedItem.Equals("Unit"))
            {
                textBox2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JourneyView JV = null;
            if (comboBox1.SelectedItem.Equals("Week"))
            {
                JV = new JourneyView(numericUpDown1.Value.ToString(), "", "");
            }
            else if (comboBox1.SelectedItem.Equals("Class"))
            {
                JV = new JourneyView("", textBox1.Text, "");
            }
            else if (comboBox1.SelectedItem.Equals("Unit"))
            {
                JV = new JourneyView("", "", textBox2.Text);
            }
            JV.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            JourneyView JV = null;
            if (comboBox1.SelectedItem.Equals("Week"))
            {
                JV = new JourneyView(numericUpDown1.Value.ToString(), "", "");
            }
            else if (comboBox1.SelectedItem.Equals("Class"))
            {
                JV = new JourneyView("", textBox1.Text, "");
            }
            else if (comboBox1.SelectedItem.Equals("Unit"))
            {
                JV = new JourneyView("", "", textBox2.Text);
            }
            label3.Text = JV.TotalMiles + " miles, " + JV.Journeys + " trips";
        }
    }
}
