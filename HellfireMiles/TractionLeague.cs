using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HellfireMiles
{
    class TractionLeague : TractionView
    {
        public TractionLeague(string classFilter = "", string comparisonSign = "", int mileThreshold = 0, string importedFrom = "") : base(classFilter, comparisonSign, mileThreshold, importedFrom)
        {
            Text = "Traction League"; //form title
            label1.Text = "";
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Class";
            dataGridView1.Columns[1].Name = "Mileage";
            dataGridView1.Columns[2].Name = "Journeys";
            dataGridView1.Columns[3].Name = "Avg Mi/Journey";
            dataGridView1.Columns[4].Name = "% Cleared";

            dataGridView1.Rows.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            comboBox1.Enabled = false;

            if (classFilter.Equals("")) //no filter applied, for display and sometimes other windows' use
            {
                JourneyView allJourneys = new JourneyView();
                foreach (var category in classes) //loop through all classes for haulages
                {
                    JourneyView jv = new JourneyView("", category, ""); //filter a JV by loco class
                    TractionView tv;
                    if (importedFrom.Equals(""))
                    { //yours
                        tv = new TractionView(category);
                    }
                    else
                    {
                        tv = new TractionView(category, "", 0, importedFrom);
                    }
                    Cleared = tv.Cleared;
                    NumLocos = tv.dataGridView1.Rows.Count;
                    double x = Math.Round((jv.TotalMiles / jv.Journeys), 2);
                    if (jv.Journeys == 0)
                    {
                        x = 0.0;
                    }
                    Object[] row = new Object[] { category, Math.Round(jv.TotalMiles, 2), jv.Journeys, x, 100 * Math.Round(tv.Cleared / (double)(tv.dataGridView1.Rows.Count - 1), 2) };
                    dataGridView1.Rows.Add(row);
                }
            }
            else //for interal use only by other windows, e.g. for comparison
            {
                JourneyView jv = new JourneyView("", classFilter, "");
                TractionView tv;
                if (importedFrom.Equals(""))
                { //yours
                    tv = new TractionView(classFilter);
                }
                else
                {
                    tv = new TractionView(classFilter, "", 0, importedFrom);
                }
                Cleared = tv.Cleared;
                NumLocos = tv.dataGridView1.Rows.Count;
                double x = Math.Round((jv.TotalMiles / jv.Journeys), 2);
                if (jv.Journeys == 0)
                {
                    x = 0.0;
                }
                Object[] row = new Object[] { classFilter, Math.Round(jv.TotalMiles, 2), jv.Journeys, x, 100 * Math.Round(tv.Cleared / (double)(tv.dataGridView1.Rows.Count - 1), 2) };
                dataGridView1.Rows.Add(row);
            }
        }
    }
}
