using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HellfireMiles
{
    public partial class JourneyView : Form
    {
        string week, trainclass, loco; //filters (for refreshing)
        string[] classes = { "03", "06", "08", "09", "13", "20", "25", "26", "27", "31", "33", "37", "40", "45", "46", "47", "50", "55", "56", "73", "76", "81", "82", "83", "85", 
            "86", "87", "98" }; //a list of classes to use for CompareStats
        Thread[] threads; //collection of threads to execute for CompareStats
        public double TotalMiles
        { get; set; }
        public double Journeys
        { get; set; }

        /// <summary>
        /// Initializes an object of the JourneyView class that takes optional parameters to filter out certain journeys from all loaded Hellfire .CSV files.
        /// </summary>
        /// <param name="weekFilter">Show all journeys from only one week, specified by user. If "", does not filter by week.</param>
        /// <param name="classFilter">Show all journeys with haulage from only one class, specified by user. If "", does not filter by class.</param>
        /// <param name="locoFilter">Show all journeys with haulage from only one loco, specified by user. If "", does not filter by loco.</param>
        public JourneyView(string weekFilter = "", string classFilter = "", string locoFilter = "")
        {
            week = weekFilter;
            trainclass = classFilter;
            loco = locoFilter;
            InitializeComponent();
            fillTable(false, false, weekFilter, classFilter, locoFilter);
        }

        public void fillTable(bool trainOnly, bool locoOnly, string weekFilter, string classFilter, string locoFilter)
        {
            string csvLocation = ""; //where the .csv files are stored
            int weekno = 0; //for table purposes
            Journeys = 0;
            TotalMiles = 0;
            //putting in the columns
            dataGridView1.ColumnCount = 11;
            string[] gridNames = { "Week #", "Day", "Loco1", "Loco2", "Loco3", "Loco4", "From", "To", "Headcode", "Train", "Mileage" };
            for (int i = 0; i < gridNames.Length; i++)
            {
                dataGridView1.Columns[i].Name = gridNames[i];
            }

            //find the directory inputted by user, located in user's AppData
            using (StreamReader sr = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles\\dir.txt"))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    csvLocation = s;
                }
            }

            //set tooltip of button7 to display the current dir
            ToolTip tt1 = new ToolTip();
            tt1.SetToolTip(button7, "The current moves list folder is " + csvLocation);
            tt1.InitialDelay = 0;

            //sort all .csvs by date created and add them to an array for reference
            //this is because by defualt they are sorted alphabetically, leading to week #100 and beyond being put before week #20, etc.
            var csvSorted = new DirectoryInfo(csvLocation).GetFiles("*.csv").OrderBy(f => f.LastWriteTime).ToArray();
            List<string> csv = new List<string>();
            foreach (FileInfo fi in csvSorted)
            {
                //add csv files into the csv array in order by last modified time, so weeks are sequential
                csv.Add(csvLocation + "\\" + fi.ToString());
            }
            foreach (var movesList in csv)
            {
                weekno++; //iterate through each week
                using (StreamReader sr = new StreamReader(movesList))
                {
                    bool fourLocos = false;
                    string content = File.ReadAllText(movesList);
                    if (content.Contains("Loco4")) //older versions have only 3 locos
                    {
                        fourLocos = true;
                    }
                    int iter = 0; //journey number per week
                    string currentLine;
                    // currentLine will be null when the StreamReader reaches the end of file
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        if (iter == 0)
                        {
                            iter++;
                            continue;
                        }
                        string dow = substringFromTo(currentLine, 0, indexOfNth(currentLine, ",", 0)); //day of week
                        string loco1 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 0) + 1, indexOfNth(currentLine, ",", 1)); //loco1
                        string loco2 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 1) + 1, indexOfNth(currentLine, ",", 2)); //loco2
                        string loco3 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 2) + 1, indexOfNth(currentLine, ",", 3)); //loco3
                        string loco4 = "", from, to, hc, train;
                        string m = "";
                        double miles = 0.0;
                        if (fourLocos)
                        {
                            loco4 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //loco4
                            from = substringFromTo(currentLine, indexOfNth(currentLine, ",", 4) + 1, indexOfNth(currentLine, ",", 5)); //from
                            to = substringFromTo(currentLine, indexOfNth(currentLine, ",", 5) + 1, indexOfNth(currentLine, ",", 6)); //to
                            hc = substringFromTo(currentLine, indexOfNth(currentLine, ",", 6) + 1, indexOfNth(currentLine, ",", 7)); //headcode
                            train = substringFromTo(currentLine, indexOfNth(currentLine, ",", 7) + 1, indexOfNth(currentLine, ",", 8)); //train
                            m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                        } else
                        {
                            from = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //from
                            to = substringFromTo(currentLine, indexOfNth(currentLine, ",", 4) + 1, indexOfNth(currentLine, ",", 5)); //to
                            hc = substringFromTo(currentLine, indexOfNth(currentLine, ",", 5) + 1, indexOfNth(currentLine, ",", 6)); //headcode
                            train = substringFromTo(currentLine, indexOfNth(currentLine, ",", 6) + 1, indexOfNth(currentLine, ",", 7)); //train
                            m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 8) + 1, currentLine.Length); //miles
                        }

                        if (currentLine.Length > 10) //is it just the name? if so, skip the line
                        {
                            if (trainOnly && (loco1.Equals("Bus") || loco1.Equals("Taxi") || loco1.Equals("Walk") || loco1.Equals("Ship"))) //rail moves only, and this is not a loco
                            {
                                continue;
                            }
                            if (locoOnly && loco1.Any(x => char.IsLetter(x))) //locos only, check if loco1 is a DMU/EMU then skip if it is
                            {
                                continue;
                            }
                            if (!weekFilter.Equals("") || !classFilter.Equals("") || !locoFilter.Equals(""))
                            { //any filters on? 
                                //disable all buttons on the form
                                //fix this eventually; for loop iterators wouldn't work here for some reason
                                button1.Enabled = false;
                                button2.Enabled = false;
                                button3.Enabled = false;
                                button4.Enabled = false;
                                button5.Enabled = false;
                                button6.Enabled = false;
                                button7.Enabled = false;
                                if (!weekFilter.Equals("")) //sorting by week?
                                {
                                    if (!weekno.ToString().Equals(weekFilter))
                                    {
                                        continue; //does not match this enabled filter, skip the line
                                    }
                                }
                                if (!classFilter.Equals("")) //sorting by class?
                                {
                                    if (fourLocos)
                                    {
                                        if (!loco1.Substring(0, 2).Equals(classFilter) && !loco2.Substring(0, 2).Equals(classFilter) && !loco3.Substring(0, 2).Equals(classFilter) && !loco4.Substring(0, 2).Equals(classFilter)) //check all possible locos for the first 3 numbers of the class
                                        {
                                            continue; //does not match this enabled filter, skip the line
                                        }
                                    } else
                                    {
                                        if (!loco1.Substring(0, 2).Equals(classFilter) && !loco2.Substring(0, 2).Equals(classFilter) && !loco3.Substring(0, 2).Equals(classFilter)) //check all possible locos for the first 3 numbers of the class
                                        {
                                            continue; //does not match this enabled filter, skip the line
                                        }
                                    }
                                }
                                if (!locoFilter.Equals("")) //sorting by locomotive?
                                {
                                    if (fourLocos)
                                    {
                                        if (!loco1.Equals(locoFilter) && !loco2.Equals(locoFilter) && !loco3.Equals(locoFilter) && !loco4.Equals(locoFilter)) //check all possible locomotives
                                        {
                                            continue; //does not match this enabled filter, skip the line
                                        }
                                    }
                                    else
                                    {
                                        if (!loco1.Equals(locoFilter) && !loco2.Equals(locoFilter) && !loco3.Equals(locoFilter)) //check all possible locomotives
                                        {
                                            continue;
                                        }

                                    }
                                }
                                //if we're here, all filters match for this move, calculate mileage, etc.
                                if (!m.Equals("None"))
                                {
                                    miles = double.Parse(m);
                                    Journeys++;
                                }
                                else //placeholder move
                                {
                                    miles = double.Parse("0.0");
                                }
                                TotalMiles += miles;
                                Object[] row = new object[] { weekno, dow, loco1, loco2, loco3, loco4, from, to, hc, train, miles };
                                dataGridView1.Rows.Add(row);
                            }
                            else
                            {
                                if (!m.Equals("None") && !m.Equals(","))
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
                                        MessageBox.Show("An error occured while loading the moves list.\nBad string: " + m + "\nThe program will attempt to load the other moves but errors may occur.");
                                    }
                                    Journeys++;
                                }
                                else //placeholder move
                                {
                                    miles = double.Parse("0.0");
                                }
                                TotalMiles += miles;
                                Object[] row = new object[] { weekno, dow, loco1, loco2, loco3, loco4, from, to, hc, train, miles };
                                dataGridView1.Rows.Add(row);
                            }
                            iter++;
                        }
                    }
                }
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    try
                    {
                        if (cell.Value.Equals("None"))
                        {
                            cell.Value = "";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            label1.Text = Journeys + " journeys, " + Decimal.Round((decimal)TotalMiles, 2) + " miles";
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
                return "";
            }
        }

        /// <summary>
        /// Opens the filter menu.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            FilterMenu f = new FilterMenu();
            f.Show();
        }

        /// <summary>
        /// Opens the traction view via a thread.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => loadTV()); //load the window in the background, so the main window won't freeze
        }

        /// <summary>
        /// Launches the traction view.
        /// </summary>
        private void loadTV()
        {
            button2.Invoke(new MethodInvoker(delegate { button2.Text = "Loading"; }));
            button2.Invoke(new MethodInvoker(delegate { button2.Enabled = false; }));
            TractionView tv = new TractionView();
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
            t.Join(); //wait for window closure
            button2.Invoke(new MethodInvoker(delegate { button2.Text = "Traction View"; }));
            button2.Invoke(new MethodInvoker(delegate { button2.Enabled = true; }));
        }

        /// <summary>
        /// Opens the export menu/a file browser.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "HellfireMiles Data|*.hfm"; //what to display on the file dialog
            saveFileDialog1.Title = "Export Moves";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK) //user saved a file
            {
                string name = Interaction.InputBox("Enter your name:", "", "");
                string path = saveFileDialog1.FileName;

                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    int iter = 0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (iter == dataGridView1.Rows.Count - 1)
                        {
                            break;
                        }
                        //write the contents of the JourneyView row to the .hfm file
                        sw.WriteLine(row.Cells["Week #"].Value + "," + row.Cells["Day"].Value + "," + row.Cells["Loco1"].Value + "," + row.Cells["Loco2"].Value + "," +
                            row.Cells["Loco3"].Value + ",," + row.Cells["From"].Value + "," + row.Cells["To"].Value + "," + row.Cells["Headcode"].Value + "," +
                            row.Cells["Train"].Value + "," + row.Cells["Mileage"].Value); //writes all data in each line
                        iter++;
                    }
                    sw.WriteLine(name); //write inputted name on the bottom of the file
                    MessageBox.Show("Moves list exported as: " + saveFileDialog1.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Opens the import menu/a file browser to import .hfm data in a child window.
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.Filter = "HellfireMiles Data|*.hfm";
            saveFileDialog1.Title = "Import Moves";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                new ImportedJourneyView("", "", "", saveFileDialog1.FileName).Show();
            }
        }

        /// <summary>
        /// Opens a file browser to compare statistics between two users, using .hfm data
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.Filter = "HellfireMiles Data|*.hfm";
            saveFileDialog1.Title = "Import Moves";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                Task.Factory.StartNew(() => loadCS(saveFileDialog1.FileName)); //load the window in the background, so the main window won't freeze
            }
        }

        /// <summary>
        /// Opens a file browser to compare loco mileages between two users, using .hfm data
        /// </summary>
        private void button6_Click_1(object sender, EventArgs e) //compare mileages
        {
            OpenFileDialog saveFileDialog1 = new OpenFileDialog();
            saveFileDialog1.Filter = "HellfireMiles Data|*.hfm";
            saveFileDialog1.Title = "Import Moves";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                Task.Factory.StartNew(() => loadCM(saveFileDialog1.FileName)); //load the window in the background, so the main window won't freeze
            }
        }

        /// <summary>
        /// Load the CompareStats window for statistics
        /// </summary>
        /// <param name="path">The .hfm file to load.</param>
        private void loadCS(string path)
        {
            button5.Invoke(new MethodInvoker(delegate { button5.Text = "Loading"; }));
            button5.Invoke(new MethodInvoker(delegate { button5.Enabled = false; }));
            CompareStats cs = new CompareStats(path, true);
            threads = new Thread[classes.Length];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(delegate ()
                {
                    cs.addClassInfo(classes[i], path);
                });
                threads[i].Start();
                System.Diagnostics.Debug.Print("class " + classes[i] + " thread started");
                Thread.Sleep(150);
            }
            foreach (var thread in threads)
            {
                thread.Join(); //wait for all loco threads to finish before loading window
            }
            Thread t = new Thread(delegate ()
            {
                try
                {
                    Application.Run(cs);
                }
                catch (ObjectDisposedException)
                {
                    //ignore
                }
            });
            t.Start();
            t.Join(); //wait for window closure
            button5.Invoke(new MethodInvoker(delegate { button5.Text = "Compare Stats"; }));
            button5.Invoke(new MethodInvoker(delegate { button5.Enabled = true; }));
        }

        /// <summary>
        /// Load the CompareStats window for mileages
        /// </summary>
        /// <param name="path">The .hfm file to load.</param>
        private void loadCM(string path)
        {
            button6.Invoke(new MethodInvoker(delegate { button6.Text = "Loading"; }));
            button6.Invoke(new MethodInvoker(delegate { button6.Enabled = false; }));
            CompareStats cs = new CompareStats(path, false);
            Thread t = new Thread(delegate ()
            {
                cs.addClassInfo();
            });
            t.Start();
            t.Join(); //wait for loco threads to finish before loading window
            Thread th = new Thread(delegate ()
            {
                try
                {
                    Application.Run(cs);
                } catch (ObjectDisposedException)
                {
                    //ignore
                }
                
            });
            th.Start();
            th.Join(); //wait for window closure
            button6.Invoke(new MethodInvoker(delegate { button6.Text = "Compare Mileages"; }));
            button6.Invoke(new MethodInvoker(delegate { button6.Enabled = true; }));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            fillTable(comboBox1.SelectedItem.ToString().Contains("Rail"), comboBox1.SelectedItem.ToString().Contains("Loco"), week, trainclass, loco);
        }

        private void button7_Click(object sender, EventArgs e) //change moves list directory
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            string movesdir;
            string dirfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\HellfireMiles\\dir.txt";
            if (result == DialogResult.OK)
            {
                movesdir = @folderBrowserDialog1.SelectedPath;
                if (movesdir != null) //a folder was actually chosen
                {
                    File.WriteAllText(dirfile, String.Empty); //clear current dir
                    using (StreamWriter sw = File.CreateText(dirfile))
                    {
                        sw.WriteLine(movesdir);
                    }
                    MessageBox.Show("Setup completed, please re-run the program to access the updated moves list.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                } else
                {
                    MessageBox.Show("You did not choose a folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}