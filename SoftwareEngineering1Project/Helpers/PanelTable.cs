using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Helpers
{
    public class PanelTable
    {
        public string title { get; set; }
        public int itemsPerRow { get; set; }
        public Dictionary<string, string> data { get; set; }
        public Dictionary<string, string> tableButtons { get; set; }

        private const string KEY_WIDTH = "20%";
        private const string VALUE_WIDTH = "30%";

        public PanelTable()
        {
            this.title = "Panel Table";
            this.itemsPerRow = 4;
            this.data = new Dictionary<string, string>();
            this.tableButtons = new Dictionary<string, string>();
        }

        public PanelTable setTitle(string title)
        {
            this.title = title;
            return this;
        }

        public PanelTable setItemsPerRow(int itemsPerRow)
        {
            this.itemsPerRow = itemsPerRow;
            return this;
        }

        public PanelTable setData(Dictionary<string, string> data)
        {
            this.data = data;
            return this;
        }

        public PanelTable setTableButtons(Dictionary<string, string> tableButtons)
        {
            this.tableButtons = tableButtons;
            return this;
        }

        public HtmlString render()
        {
            string html = "<div class='panel panel-primary table table-responsive' style='width:70%;margin:auto;'>";//open the panel
            html += "<div class='panel-heading'>" + this.title + "</div>";//set the panel heading to the title
            html += "<table class='table table-responsive'>";//open up the table

            int count = 0;//set a counter to keep up with rows
            foreach (KeyValuePair<string,string> entry in this.data)
            {
                if (count % (this.itemsPerRow / 2) == 0)
                {
                    html += "<tr>";
                }

                //add each data entry to the table
                html += "<th width='" + KEY_WIDTH + "'>" + entry.Key + "</th>";
                html += "<td width='" + VALUE_WIDTH + "'>" + entry.Value + "</td>";

                if (count % (this.itemsPerRow / 2) == 1)
                {
                    html += "</tr>";
                }

                count++;
            }

            //if there are any table buttons then add them to the table
            if (this.tableButtons.Count > 0)
            {
                html += "<tr class='text-center'>";//create a row with centered text
                html += "<td colspan='" + this.itemsPerRow + "'>";//make the row the colspan of the table

                //add each button to the table
                foreach (KeyValuePair<string, string> entry in this.tableButtons)
                {
                    html += "<a class='btn btn-primary btn-sm' href='" + entry.Key + "'>" + entry.Value + "</a>";
                }

                html += "</td>";
                html += "</tr>";
            }

            html += "</table>";//close the table
            html += "</div>";//close panel wrapper


            return new HtmlString(html);
        }
    }
}