using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Helpers
{
    public class PanelTable
    {
        public string Title { get; set; }
        public int ItemsPerRow { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public Dictionary<string, string> TableButtons { get; set; }

        private const string KEY_WIDTH = "20%";
        private const string VALUE_WIDTH = "30%";

        public PanelTable()
        {
            Title = "Panel Table";
            this.ItemsPerRow = 4;
            this.Data = new Dictionary<string, string>();
            this.TableButtons = new Dictionary<string, string>();
        }

        public PanelTable setTitle(string title)
        {
            this.Title = title;
            return this;
        }

        public PanelTable setItemsPerRow(int itemsPerRow)
        {
            this.ItemsPerRow = itemsPerRow;
            return this;
        }

        public PanelTable setData(Dictionary<string, string> data)
        {
            this.Data = data;
            return this;
        }

        public PanelTable setTableButtons(Dictionary<string, string> tableButtons)
        {
            this.TableButtons = tableButtons;
            return this;
        }

        public HtmlString Render()
        {
            string html = "<div class='panel panel-primary table table-responsive' style='width:70%;margin:auto;'>";//open the panel
            html += "<div class='panel-heading'>" + this.Title + "</div>";//set the panel heading to the title
            html += "<table class='table table-responsive'>";//open up the table

            int count = 0;//set a counter to keep up with rows
            foreach (KeyValuePair<string,string> entry in this.Data)
            {
                if (count % (this.ItemsPerRow / 2) == 0)
                {
                    html += "<tr>";
                }

                //add each data entry to the table
                html += "<th width='" + KEY_WIDTH + "'>" + entry.Key + "</th>";
                html += "<td width='" + VALUE_WIDTH + "'>" + entry.Value + "</td>";

                if (count % (this.ItemsPerRow / 2) == 1)
                {
                    html += "</tr>";
                }

                count++;
            }

            //if there are any table buttons then add them to the table
            if (this.TableButtons.Count > 0)
            {
                html += "<tr class='text-center'>";//create a row with centered text
                html += "<td colspan='" + this.ItemsPerRow + "'>";//make the row the colspan of the table

                //add each button to the table
                foreach (KeyValuePair<string, string> entry in this.TableButtons)
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