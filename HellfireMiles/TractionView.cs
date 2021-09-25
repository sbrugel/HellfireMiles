using HellfireMiles.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        List<string> classes = new List<string>();
        public TractionView(bool traction, string classFilter, string importedFrom)
        {
            InitializeComponent();
            if (traction)
            {
                label1.Text = "";
                dataGridView1.ColumnCount = 5;
                dataGridView1.Columns[0].Name = "Class";
                dataGridView1.Columns[1].Name = "Mileage";
                dataGridView1.Columns[2].Name = "Journeys";
                dataGridView1.Columns[3].Name = "Avg Mi/Journey";
                dataGridView1.Columns[4].Name = "% Cleared";

                addClasses();

                if (classFilter.Equals(""))
                {
                    foreach (var category in classes)
                    {
                        JourneyView jv = new JourneyView("", category, "");
                        TractionView tv;
                        if (importedFrom.Equals(""))
                        { //yours
                            tv = new TractionView(false, category, "");

                        }
                        else
                        {
                            tv = new TractionView(false, category, importedFrom);
                        }
                        Cleared = tv.Cleared;
                        NumLocos = tv.dataGridView1.Rows.Count;
                        double x = Math.Round((jv.TotalMiles / jv.Journeys), 2);
                        if (jv.Journeys == 0)
                        {
                            x = 0.0;
                        }
                        Object[] row = new Object[] { category, Math.Round(jv.TotalMiles, 2), jv.Journeys, x, 100 * (tv.Cleared / (double)(tv.dataGridView1.Rows.Count - 1)) };
                        dataGridView1.Rows.Add(row);
                    }
                } else
                {
                    JourneyView jv = new JourneyView("", classFilter, "");
                    TractionView tv;
                    if (importedFrom.Equals(""))
                    { //yours
                        tv = new TractionView(false, classFilter, "");
                    }
                    else
                    {
                        tv = new TractionView(false, classFilter, importedFrom);
                    }
                    Cleared = tv.Cleared;
                    NumLocos = tv.dataGridView1.Rows.Count;
                    double x = Math.Round((jv.TotalMiles / jv.Journeys), 2);
                    if (jv.Journeys == 0)
                    {
                        x = 0.0;
                    }
                    Object[] row = new Object[] { classFilter, Math.Round(jv.TotalMiles, 2), jv.Journeys, x, 100*(tv.Cleared / (double)(tv.dataGridView1.Rows.Count - 1)) };
                    dataGridView1.Rows.Add(row);
                }
                
            } else
            {
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
                        } else
                        {
                            if (l.Substring(0, 2).Equals(classFilter))
                            {
                                Object[] row = new Object[] { l, 0.0, 0 };
                                dataGridView1.Rows.Add(row);
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
                                var miles = 0.0;
                                if (!m.Equals("None"))
                                {
                                    miles = double.Parse(m);
                                }
                                else
                                {
                                    miles = double.Parse("0.0");
                                }
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
                                if (!loco4.Equals(null))
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
                    label1.Text += ", you have Cleared " + Cleared + " (about " + Math.Round(((double)Cleared / (dataGridView1.Rows.Count - 1)) * 100, 2) + "%). ";
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
                } else //external (only for internal use)
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
                            var miles = 0.0;
                            if (!m.Equals("None"))
                            {
                                try
                                {
                                    miles = double.Parse(m);
                                } catch (Exception ex)
                                {
                                    //nothing
                                }
                                
                            }
                            else
                            {
                                miles = double.Parse("0.0");
                            }
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
                            if (!loco4.Equals(null))
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
        }

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

        public double getPercentage()
        {
            return Math.Round(((double)Cleared / (dataGridView1.Rows.Count - 1)) * 100, 2);
        }

        public void addClasses()
        {
            classes.Add("03");
            classes.Add("06");
            classes.Add("08");
            classes.Add("09");
            classes.Add("13");
            classes.Add("20");
            classes.Add("25");
            classes.Add("26");
            classes.Add("27");
            classes.Add("31");
            classes.Add("33");
            classes.Add("37");
            classes.Add("40");
            classes.Add("45");
            classes.Add("46");
            classes.Add("47");
            classes.Add("50");
            classes.Add("55");
            classes.Add("56");
            classes.Add("73");
            classes.Add("76");
            classes.Add("81");
            classes.Add("82");
            classes.Add("83");
            classes.Add("85");
            classes.Add("86");
            classes.Add("87");
            classes.Add("98");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new TractionView(true, "", "").Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new TractionView(false, textBox1.Text, "").Show();
        }
    }
}
