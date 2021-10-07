using HellfireMiles.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace HellfireMiles
{
    public partial class CompareStats : Form
    {
        string importedFile;
        Thread yourData, theirData;
        string[] classes = { "03", "06", "08", "09", "13", "20", "25", "26", "27", "31", "33", "37", "40", "45", "46", "47", "50", "55", "56", "73", "76", "81", "82", "83", "85",
            "86", "87", "98" }; //a list of classes to use for CompareStats
        public CompareStats(string imf, bool stats) //stats is true if its not individual haulages
        {
            Text = "Compare Stats"; //form title
            importedFile = imf;
            InitializeComponent();
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Category";
            dataGridView1.Columns[1].Name = "You";
            dataGridView1.Columns[2].Name = getName(importedFile);
        }
        /// <summary>
        /// Grabs mileage data per loco from the program user and an imported file.
        /// </summary>
        public void addClassInfo() //for haulages/mileage comparison only
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Loco";
            using (var reader = new StringReader(Resources.baseneeds))
            {
                string l;
                while ((l = reader.ReadLine()) != null)
                {
                    Object[] row = new Object[] { l, 0.0, 0.0 };
                    dataGridView1.Rows.Add(row);
                }
            }

            //get moves data from both people
            yourData = new Thread(getYourHaulages);
            theirData = new Thread(getTheirHaulages);
            yourData.Start();
            theirData.Start();
            yourData.Join();
            theirData.Join();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (Double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) > Double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGreen;
                }
                else if (Double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) < Double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                {
                    dataGridView1.Rows[i].Cells[2].Style.BackColor = Color.LightGreen;
                }
                System.Diagnostics.Debug.Print("still working..." + i + " of " + dataGridView1.Rows.Count);
            }
        }
        /// <summary>
        /// Fills the data grid view with statistics after grabbing data from .hfm/.csv files
        /// <param name="category">The loco class to fill data from.</param>
        /// <param name="savefile">The imported file to fill data from.</param>
        /// </summary>
        public void addClassInfo(string category, string savefile) //for stats only
        {
            System.Diagnostics.Debug.Print("0 - " + category);
            JourneyView jv = new JourneyView("", category, ""); //your stats
            TractionView tv = new TractionLeague(category, "", 0, "");
            JourneyView jv2 = new ImportedJourneyView("", category, "", savefile); //their stats
            TractionView tv2 = new TractionLeague(category, "", 0, savefile);
            //for averages only
            double x = Math.Round((jv.TotalMiles / jv.Journeys), 2);
            if (jv.Journeys == 0)
            {
                x = 0.0;
            }
            System.Diagnostics.Debug.Print("1 - " + category);

            //for averages only
            double x2 = Math.Round((jv2.TotalMiles / jv2.Journeys), 2);
            if (jv2.Journeys == 0)
            {
                x2 = 0.0;
            }
            System.Diagnostics.Debug.Print("2 - " + category);

            //add rows to the table (jv/tv are for user, jv2/tv2 are for the other person)
            Object[] row = new Object[] { "Class " + category + ": Total Mileage", Math.Round(jv.TotalMiles, 2), Math.Round(jv2.TotalMiles) };
            dataGridView1.Rows.Add(row);
            row = new Object[] { "Class " + category + ": % Cleared", Math.Round(100 * (tv.Cleared / (double)tv.NumLocos), 2), Math.Round(100 * (tv2.Cleared / (double)tv2.NumLocos), 2) };
            dataGridView1.Rows.Add(row);
            row = new Object[] { "Class " + category + ": # of Journeys", jv.Journeys, jv2.Journeys };
            dataGridView1.Rows.Add(row);
            row = new Object[] { "Class " + category + ": Avg. Mi/Journey", x, x2 };
            dataGridView1.Rows.Add(row);
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (Double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) > Double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.LightGreen;
                }
                else if (Double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()) < Double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()))
                {
                    dataGridView1.Rows[i].Cells[2].Style.BackColor = Color.LightGreen;
                }
                System.Diagnostics.Debug.Print("still working..." + i + " of " + dataGridView1.Rows.Count);
            }
        }

        /// <summary>
        /// Grabs data of moves from the moves directory.
        /// </summary>
        public void getYourHaulages()
        {
            string csvLocation = null;
            using (StreamReader sr = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles\\dir.txt"))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    csvLocation = s;
                }
            }
            var csv = Directory.GetFiles(csvLocation, "*.csv", SearchOption.AllDirectories);
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
                                dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                            }
                        }
                        if (!loco2.Equals(null) && !loco2.Equals("None"))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco2))
                                {
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                }
                            }
                        }
                        if (!loco3.Equals(null) && !loco3.Equals("None"))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco3))
                                {
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                }
                            }
                        }
                        if (!loco4.Equals(null) && !loco4.Equals("None"))
                        {
                            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco4))
                                {
                                    dataGridView1.Rows[i].Cells[1].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) + miles;
                                    dataGridView1.Rows[i].Cells[1].Value = Math.Round((double)dataGridView1.Rows[i].Cells[1].Value, 2);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Grabs data of moves from an imported file.
        /// </summary>
        public void getTheirHaulages()
        {
            using (StreamReader sr = new StreamReader(importedFile))
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
                    var loco1 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 1) + 1, indexOfNth(currentLine, ",", 2)); //loco1
                    var loco2 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 2) + 1, indexOfNth(currentLine, ",", 3)); //loco2
                    var loco3 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //loco3
                    var loco4 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 4) + 1, indexOfNth(currentLine, ",", 5)); //loco4
                    var m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                    var miles = 0.0;
                    if (!m.Equals("None"))
                    {
                        try
                        {
                            miles = double.Parse(m);
                        }
                        catch (Exception ex)
                        {

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
                            dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value) + miles;
                            dataGridView1.Rows[i].Cells[2].Value = Math.Round((double)dataGridView1.Rows[i].Cells[2].Value, 2);
                        }
                    }
                    if (!loco2.Equals(null) && !loco2.Equals("None"))
                    {
                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco2))
                            {
                                dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value) + miles;
                                dataGridView1.Rows[i].Cells[2].Value = Math.Round((double)dataGridView1.Rows[i].Cells[2].Value, 2);
                            }
                        }
                    }
                    if (!loco3.Equals(null) && !loco3.Equals("None"))
                    {
                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco3))
                            {
                                dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value) + miles;
                                dataGridView1.Rows[i].Cells[2].Value = Math.Round((double)dataGridView1.Rows[i].Cells[2].Value, 2);
                            }
                        }
                    }
                    if (!loco4.Equals(null) && !loco4.Equals("None"))
                    {
                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(loco4))
                            {
                                dataGridView1.Rows[i].Cells[2].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value) + miles;
                                dataGridView1.Rows[i].Cells[2].Value = Math.Round((double)dataGridView1.Rows[i].Cells[2].Value, 2);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Finds the name used for an imported file.
        /// </summary>
        /// <param name="importedFile">The imported moves list to scan.</param>
        /// <returns>A string representing the name the exporter used for their name.</returns>
        public string getName(string importedFile)
        {
            using (StreamReader sr = new StreamReader(importedFile))
            {
                string currentLine = "Default";
                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {
                    var m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                    var miles = 0.0;
                    //check each row for the name
                    if (!m.Equals("None"))
                    {
                        try
                        {
                            miles = double.Parse(m);
                        }
                        catch (Exception ex)
                        {
                            //name because string cannot be parsed
                            return currentLine;
                        }

                    }

                }
            }
            return "Default"; //just in case the file does not happen to ahve the name
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
            }
            catch (Exception ex)
            {
                return "hi";
            }

        }
    }
}
