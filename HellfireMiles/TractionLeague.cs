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
        public TractionLeague(string classFilter, string importedFrom) : base(classFilter, importedFrom)
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
            addClasses();
            button1.Enabled = false;
            button2.Enabled = false;
            textBox1.Enabled = false;

            if (classFilter.Equals(""))
            {
                foreach (var category in classes)
                {
                    JourneyView jv = new JourneyView("", category, "");
                    TractionView tv;
                    if (importedFrom.Equals(""))
                    { //yours
                        tv = new TractionView(category, "");
                    }
                    else
                    {
                        tv = new TractionView(category, importedFrom);
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
                    tv = new TractionView(classFilter, "");
                }
                else
                {
                    tv = new TractionView(classFilter, importedFrom);
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
