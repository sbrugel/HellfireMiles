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
        /// <summary>
        /// Shows a new JourneyView with filters applied based on filters selected.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            JourneyView JV = null;
            string weekParam, classParam, locoParam;
            if (!checkBox1.Checked)
            {
                weekParam = ""; //leave empty ("" means the filter is ignored) if checkbox not checked
            }
            else weekParam = numericUpDown1.Value.ToString();
            if (!checkBox2.Checked)
            {
                classParam = ""; //leave empty ("" means the filter is ignored) if checkbox not checked
            }
            else classParam = textBox1.Text;
            if (!checkBox3.Checked)
            {
                locoParam = ""; //leave empty ("" means the filter is ignored) if checkbox not checked
            }
            else locoParam = textBox2.Text;
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked)
            {
                MessageBox.Show("You didn't check any filters!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else
            {
                JV = new JourneyView(weekParam, classParam, locoParam);
                JV.Show();
            }
        }
        /// <summary>
        /// Displays mileages on the filter window based on filters selected.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            JourneyView JV = new JourneyView("", "", textBox2.Text);
            label4.Text = JV.TotalMiles + " miles, " + JV.Journeys + " trips";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = !numericUpDown1.Enabled; //toggle
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !textBox1.Enabled; //toggle
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = !textBox2.Enabled; //toggle
            button2.Enabled = !button2.Enabled; //toggle
        }
    }
}
