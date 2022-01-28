using HellfireMiles.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HellfireMiles
{
    public partial class TractionView : Form
    {
        string csvLocation;
        public int Cleared
        {
            get;set;
        }
        public int NumLocos
        {
            get;set;
        }
        protected string[] classes = { "03", "06", "08", "09", "13", "20", "25", "26", "27", "31", "33", "37", "40", "45", "46", "47", "50", "55", "56", "73", "76", "81", "82", "83", "85",
            "86", "87", "98" }; //a list of classes
        /// <summary>
        /// Initializes an object of the TractionView class that takes optional parameters to filter out mileage data for certain classes only.
        /// </summary>
        /// <param name="classFilter">Show all mileages with haulage from only one class, specified by user. If "", does not filter by class.</param>
        /// <param name="comparisonSign">Show all mileages with haulage that is either less/greater than than a # of miles, specified by user. If "", does not filter by mileage.</param>
        /// <param name="mileThreshold">Show all mileages with haulage based on mileage, specified by user. If 0, does not filter by mileage.</param>
        /// <param name="importedFrom">Show mileage data from another user's imported .hfm data. If "", uses user's .csv data.</param>
        public TractionView(string classFilter = "", string comparisonSign = "", double mileThreshold = 0, string importedFrom = "")
        {
            InitializeComponent();
            button2.Enabled = false; //disable by default because filter textboxes are both empty
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Loco";
            dataGridView1.Columns[1].Name = "Mileage";
            dataGridView1.Columns[2].Name = "Journeys";
            using (var reader = new StringReader(Resources.baseneeds))
            {
                string l;
                while ((l = reader.ReadLine()) != null)
                {
                    if (classFilter.Equals(""))
                    {
                        Object[] row = new Object[] { l, 0.0, 0 };
                        dataGridView1.Rows.Add(row);
                    }
                    else
                    {
                        //do not allow users to click buttons - this would probably cause an endless spiral of forms to be honest
                        button1.Enabled = false;
                        button2.Enabled = false;
                        textBox1.Enabled = false;
                        if (!classFilter.Equals("")) //class filter applied
                        {
                            if (l.Substring(0, 2).Equals(classFilter))
                            {
                                Object[] row = new Object[] { l, 0.0, 0 };
                                dataGridView1.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            string[] csv = null;
            if (importedFrom.Equals("")) //your own data
            {
                using (StreamReader sr = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles\\dir.txt"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        csvLocation = s;
                    }
                }
                csv = Directory.GetFiles(csvLocation, "*.csv", SearchOption.AllDirectories);
                foreach (var movesList in csv)
                {
                    using (StreamReader sr = new StreamReader(movesList))
                    {
                        int iter = 0;
                        string currentLine;
                        // currentLine will be null when the StreamReader reaches the end of file
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            if (iter == 0)
                            {
                                iter++;
                                continue;
                            }
                            var loco1 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 0) + 1, indexOfNth(currentLine, ",", 1)); //loco1
                            var loco2 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 1) + 1, indexOfNth(currentLine, ",", 2)); //loco2
                            var loco3 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 2) + 1, indexOfNth(currentLine, ",", 3)); //loco3
                            var loco4 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //loco4
                            var m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                            try
                            {
                                double numericCheck = double.Parse(loco4); //is the loco a number? (for older CSVs)
                            } catch (Exception ex) {
                                m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 8) + 1, currentLine.Length); //miles
                            }
                            
                            var miles = 0.0;
                            if (!m.Equals("None") && !m.Equals("X"))
                            {
                                if (m.Length > 6) //did it get the mileage and only the mileage?
                                {
                                    m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                                }
                                try
                                {
                                    miles = double.Parse(m);
                                } catch (Exception ex)
                                {
                                    //nada
                                }
                            }
                            else
                            {
                                miles = double.Parse("0.0");
                            }
                            //look for cleared locos in all positions
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco1))
                                {
                                    editRow(i, loco1, miles);
                                }
                            }
                            if (!loco2.Equals(null))
                            {
                                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                                {
                                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco2))
                                    {
                                        editRow(i, loco2, miles);
                                    }
                                }
                            }
                            if (!loco3.Equals(null))
                            {
                                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                                {
                                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco3))
                                    {
                                        editRow(i, loco3, miles);
                                    }
                                }
                            }
                            if (!loco4.Equals(null) || !loco4.Equals("X"))
                            {
                                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                                {
                                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco4))
                                    {
                                        editRow(i, loco4, miles);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.LightGreen)
                    {
                        Cleared++;
                    }
                }
                if (mileThreshold == 0) //no mile filter, so show the bonus message
                {
                    label1.Text = "Out of " + (dataGridView1.Rows.Count - 1) + " locos of class ";
                    if (classFilter.Equals("")) //no filter
                    {
                        label1.Text += "ALL";
                    }
                    else label1.Text += classFilter;
                    label1.Text += ", you have Cleared " + Cleared + " (about " + Math.Round(((double)Cleared / (dataGridView1.Rows.Count - 1)) * 100, 2) + "%). ";
                    switch ((int)(((double)Cleared / (dataGridView1.Rows.Count - 1)) * 10)) //set bonus message by 10% increments
                    {
                        case 10:
                            label1.Text += "Congratulations, that's all of them!";
                            break;
                        case 9:
                        case 8:
                            label1.Text += "Almost there! Don't lose hope!";
                            break;
                        case 7:
                        case 6:
                        case 5:
                            label1.Text += "You're just above halfway, keep hunting!";
                            break;
                        case 4:
                        case 3:
                            label1.Text += "You've got a few, there's still more out there!";
                            break;
                        case 2:
                        case 1:
                            label1.Text += "You're just getting started, there's more locos waiting for you!";
                            break;
                        default:
                            label1.Text += "Never a better time to get started!";
                            break;
                    }
                } else //mile filter, delete irrelevant rows
                {
                    label1.Text = "";
                    List<DataGridViewRow> toDelete = new List<DataGridViewRow>();
                    if (dataGridView1.Rows.Count > 1)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (comparisonSign.Equals(">"))
                            {
                                try
                                {
                                    if ((double)row.Cells[1].Value < mileThreshold)
                                    {
                                        toDelete.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }
                            else if (comparisonSign.Equals("<"))
                            {
                                try
                                {
                                    if ((double)row.Cells[1].Value > mileThreshold)
                                    {
                                        toDelete.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //ignore
                                }
                            }
                            else
                            {
                                //ignore
                            }
                        }
                        foreach (DataGridViewRow row in toDelete)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                }
            }
            else //external (only for internal use)
            {
                using (StreamReader sr = new StreamReader(importedFrom))
                {
                    int iter = 0;
                    string currentLine;
                    // currentLine will be null when the StreamReader reaches the end of file
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        if (iter == 0)
                        {
                            iter++;
                            continue;
                        }
                        var loco1 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 0) + 1, indexOfNth(currentLine, ",", 1)); //loco1
                        var loco2 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 1) + 1, indexOfNth(currentLine, ",", 2)); //loco2
                        var loco3 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 2) + 1, indexOfNth(currentLine, ",", 3)); //loco3
                        var loco4 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //loco4
                        var m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                        try
                        {
                            double numericCheck = double.Parse(loco4); //is the loco a number? (for older CSVs)
                        }
                        catch (Exception ex)
                        {
                            m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 8) + 1, currentLine.Length); //miles
                        }
                        var miles = 0.0;
                        if (!m.Equals("None") && !m.Equals("X"))
                        {
                            if (m.Length > 6) //did it get the mileage and only the mileage?
                            {
                                m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                            }
                            try
                            {
                                miles = double.Parse(m);
                            }
                            catch (Exception ex)
                            {
                                //nothing
                            }
                        }
                        else
                        {
                            miles = double.Parse("0.0");
                        }
                        //check if loco is in either of the four loco positions
                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco1))
                            {
                                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                                dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                dataGridView1.Rows[i].Cells[2].Value = (int)dataGridView1.Rows[i].Cells[2].Value + 1;
                            }
                        }
                        if (!loco2.Equals(null))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco2))
                                {
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                    dataGridView1.Rows[i].Cells[2].Value = (int)dataGridView1.Rows[i].Cells[2].Value + 1;
                                }
                            }
                        }
                        if (!loco3.Equals(null))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco3))
                                {
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                    dataGridView1.Rows[i].Cells[2].Value = (int)dataGridView1.Rows[i].Cells[2].Value + 1;
                                }
                            }
                        }
                        if (!loco4.Equals(null) || !loco4.Equals("X"))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco4))
                                {
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                    dataGridView1.Rows[i].Cells[2].Value = (int)dataGridView1.Rows[i].Cells[2].Value + 1;
                                }
                            }
                        }
                    }
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.LightGreen)
                    {
                        Cleared++;
                    }
                }
                label1.Text = "Out of " + (dataGridView1.Rows.Count - 1) + " locos of class ";
                if (classFilter.Equals(""))
                {
                    label1.Text += "ALL";
                }
                else label1.Text += classFilter;
                label1.Text += ", you have Cleared " + Cleared + " (about " + Math.Round((double)Cleared / (dataGridView1.Rows.Count - 1) * 100, 2) + "%). ";
                switch ((int)(((double)Cleared / (dataGridView1.Rows.Count - 1)) * 10))
                {
                    case 10:
                        label1.Text += "Congratulations, that's all of them!";
                        break;
                    case 9:
                    case 8:
                        label1.Text += "Almost there! Don't lose hope!";
                        break;
                    case 7:
                    case 6:
                    case 5:
                        label1.Text += "You're just above halfway, keep hunting!";
                        break;
                    case 4:
                    case 3:
                        label1.Text += "You've got a few, there's still more out there!";
                        break;
                    case 2:
                    case 1:
                        label1.Text += "You're just getting started, there's more locos waiting for you!";
                        break;
                    default:
                        label1.Text += "Never a better time to get started!";
                        break;
                }
            }
        }
        /// <summary>
        /// Finds the nth index of a substring in a string.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="value">The substring to look for.</param>
        /// <param name="nth">What occurence of the substring to find.</param>
        /// <returns>An integer corresponding to the position of the nth index of value in str</returns>
        public static int indexOfNth(string str, string value, int nth = 0)
        {
            if (nth < 0)
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");

            int offset = str.IndexOf(value);
            for (int i = 0; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }
        /// <summary>
        /// In complement with the above, finds substring between indices (if that's how the plural is spelt!)
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="from">The starting index of the string to search</param>
        /// <param name="to">The ending index of the string, stop searching here</param>
        /// <returns>The substring of str from the start to end index specified by the code</returns>
        public string substringFromTo(string str, int from, int to)
        {
            try
            {
                if (str.Substring(from, to - from).Equals(""))
                {
                    return "None";
                }
                return str.Substring(from, to - from);
            } catch (Exception ex)
            {
                return "None";
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => loadTL()); //load the TractionLeague window in the background, so the main window won't freeze
        }
        /// <summary>
        /// Loads a TractionLeague window
        /// </summary>
        private void loadTL()
        {
            button1.Invoke(new MethodInvoker(delegate { button1.Text = "Loading"; }));
            button1.Invoke(new MethodInvoker(delegate { button1.Enabled = false; }));
            TractionLeague tv = new TractionLeague();
            Thread t = new Thread(delegate ()
            {
                try
                {
                    Application.Run(tv);
                }
                catch (ObjectDisposedException)
                {
                    //ignore since form is closed
                }
            });
            t.Start();
            t.Join();
            button1.Invoke(new MethodInvoker(delegate { button1.Text = "Traction League"; }));
            button1.Invoke(new MethodInvoker(delegate { button1.Enabled = true; }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => loadTVFilter()); //load the window in the background, so the main window won't freeze
        }
        /// <summary>
        /// Loads a TractionView window, filtered by class
        /// </summary>
        private void loadTVFilter()
        {
            string classname = textBox1.Text;
            button2.Invoke(new MethodInvoker(delegate { button2.Text = "Loading"; }));
            button2.Invoke(new MethodInvoker(delegate { button2.Enabled = false; }));
            TractionView tv = null;
            if (textBox2.Text == "")
            {
                textBox2.Invoke(new MethodInvoker(delegate { textBox2.Text = "-1"; }));
                comboBox1.Invoke(new MethodInvoker(delegate { comboBox1.SelectedItem = ">"; }));
            }
            comboBox1.Invoke(new MethodInvoker(delegate { tv = new TractionView(textBox1.Text, (string)comboBox1.SelectedItem, Int32.Parse(textBox2.Text), ""); }));
            Thread t = new Thread(delegate ()
            {
                try
                {
                    Application.Run(tv);
                }
                catch (ObjectDisposedException)
                {
                    //ignore since form is closed
                }
            });
            if (textBox2.Text == "-1")
            {
                textBox2.Invoke(new MethodInvoker(delegate { textBox2.Text = ""; }));
                comboBox1.Invoke(new MethodInvoker(delegate { comboBox1.SelectedItem = ""; }));
            }
            foreach (DataGridViewRow row in tv.dataGridView1.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.LightGreen)
                {
                    Cleared++;
                }
            }
            if (textBox2.Text == "-1") //do not show % cleared with a mileage filter
            {
                tv.label1.Text = "Out of " + (tv.dataGridView1.Rows.Count - 1) + " locos of class " + classname + ", you have Cleared " + tv.Cleared + " (about " + Math.Round(((double)tv.Cleared / (tv.dataGridView1.Rows.Count - 1)) * 100, 2) + "%). ";
            }
            t.Start();
            t.Join();
            button2.Invoke(new MethodInvoker(delegate { button2.Text = "Sort by Class:"; }));
            button2.Invoke(new MethodInvoker(delegate { button2.Enabled = true; }));
        }

        private void editRow(int row, string loco, double miles)
        {
            if (dataGridView1.Rows[row].Cells[0].Value.ToString().Equals(loco))
            {
                dataGridView1.Rows[row].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridView1.Rows[row].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[row].Cells[1].Value) + miles;
                dataGridView1.Rows[row].Cells[1].Value = Math.Round((double)dataGridView1.Rows[row].Cells[1].Value, 2);
                dataGridView1.Rows[row].Cells[2].Value = (int)dataGridView1.Rows[row].Cells[2].Value + 1;
            }
        }

        /// <summary>
        /// Checks if textBox1/textBox2 (class/mileage filter) are non-empty; if so, enables the Traction View filter button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = (!textBox1.Text.Equals("") || !textBox2.Text.Equals(""));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = (!textBox1.Text.Equals("") || !textBox2.Text.Equals(""));
        }
    }
}
