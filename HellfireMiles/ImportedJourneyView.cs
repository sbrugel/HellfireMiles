using System;
using System.IO;

namespace HellfireMiles
{
    class ImportedJourneyView : JourneyView
    {
        public ImportedJourneyView(string weekFilter, string classFilter, string locoFilter, string importedFrom) : base (weekFilter, classFilter, locoFilter)
        {
            Text = "Hellfire Miles"; //form title
            //disable all buttons on the form
            //fix this eventually; for loop iterators wouldn't work here for some reason
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;

            dataGridView1.Rows.Clear();
            Journeys = 0;
            TotalMiles = 0;
            using (StreamReader sr = new StreamReader(importedFrom))
            {
                int iter = 0;
                string currentLine, name = "Default";

                /*
                * current line reads the full line of the .hfm file
                * name - this is the user's name, inputted when exporting their data
                */

                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (iter == 0)
                    {
                        iter++;
                        continue;
                    }

                    var weekno = substringFromTo(currentLine, 0, indexOfNth(currentLine, ",", 0)); //day of week, this is within the .hfm file so no need for an iteration
                    var dow = substringFromTo(currentLine, indexOfNth(currentLine, ",", 0) + 1, indexOfNth(currentLine, ",", 1)); //day of week
                    var loco1 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 1) + 1, indexOfNth(currentLine, ",", 2)); //loco1
                    var loco2 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 2) + 1, indexOfNth(currentLine, ",", 3)); //loco2
                    var loco3 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 3) + 1, indexOfNth(currentLine, ",", 4)); //loco3
                    var loco4 = substringFromTo(currentLine, indexOfNth(currentLine, ",", 4) + 1, indexOfNth(currentLine, ",", 5)); //loco4
                    var from = substringFromTo(currentLine, indexOfNth(currentLine, ",", 5) + 1, indexOfNth(currentLine, ",", 6)); //from
                    var to = substringFromTo(currentLine, indexOfNth(currentLine, ",", 6) + 1, indexOfNth(currentLine, ",", 7)); //to
                    var hc = substringFromTo(currentLine, indexOfNth(currentLine, ",", 7) + 1, indexOfNth(currentLine, ",", 8)); //headcode
                    var train = substringFromTo(currentLine, indexOfNth(currentLine, ",", 8) + 1, indexOfNth(currentLine, ",", 9)); //train
                    var m = substringFromTo(currentLine, indexOfNth(currentLine, ",", 9) + 1, currentLine.Length); //miles
                    var miles = 0.0;

                    if (!classFilter.Equals("")) //is there a class filter?
                    {
                        try
                        {
                            if (loco1.Substring(0, 2).Equals(classFilter))
                            {
                                if (!m.Equals("None")) //not a placeholder move, e.g. walking?
                                {
                                    miles = double.Parse(m);
                                    Journeys++;
                                }
                                else
                                {
                                    miles = double.Parse("0.0");
                                }
                                TotalMiles += miles; //add the miles from this move
                                Object[] row = new object[] { weekno, dow, loco1, loco2, loco3, loco4, from, to, hc, train, miles };
                                dataGridView1.Rows.Add(row); //add the row of this move
                            }
                        } catch (Exception ex)
                        {
                            //just skip the line
                        }
                    }
                    else //no filter
                    {
                        if (!m.Equals("None")) //not a placeholder move, e.g. walking?
                        {
                            try
                            {
                                miles = double.Parse(m);
                                Journeys++;
                            }
                            catch (Exception ex)
                            {
                                //name so do nothing
                                name = currentLine;
                            }

                        }
                        else
                        {
                            miles = double.Parse("0.0");
                        }
                        TotalMiles += miles; //add the miles from this move
                        Object[] row = new object[] { weekno, dow, loco1, loco2, loco3, loco4, from, to, hc, train, miles };
                        dataGridView1.Rows.Add(row); //add the row of this move
                    }
                    iter++;
                    label1.Text = name + " has been on " + Journeys + " journeys, covering a total of " + Decimal.Round((decimal)TotalMiles, 2) + " miles!";
                    button4.Enabled = false;
                }
            }
        }
    }
}
