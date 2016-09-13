using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Helpers
{
    public class DataTableModel
    {
        public string title { get; set; }
        public List<object> headers { get; set; }
        public bool searchSort { get; set; }
        public List<object> data { get; set; }
        public List<object> actions { get; set; }
        public List<object> tableButtons { get; set; }

        public DataTableModel()
        {
            title = "Data Table";
            searchSort = false;
            headers = new List<object>();
            data = new List<object>();
            actions = new List<object>();
            tableButtons = new List<object>();
        }

        public DataTableModel(string title, bool searchSort, List<object> headers, List<object> data, List<object> actions, List<object> tableButtons)
        {
            this.title = title;
            this.searchSort = searchSort;
            this.headers = headers;
            this.data = data;
            this.actions = actions;
            this.tableButtons = tableButtons;
        }

        public DataTableModel setTitle(string title)
        {
            this.title = title;
            return this;
        }

        public DataTableModel setSearchSort(bool searchSort)
        {
            this.searchSort = searchSort;
            return this;
        }

        public DataTableModel setHeaders(List<object> headers)
        {
            this.headers = headers;
            return this;
        }

        public DataTableModel setData(List<object> data)
        {
            this.data = data;
            return this;
        }

        public DataTableModel setActions(List<object> actions)
        {
            this.actions = actions;
            return this;
        }

        public DataTableModel setTableButtons(List<object> tableButtons)
        {
            this.tableButtons = tableButtons;
            return this;
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public HtmlString render()
        {
            string tableID = randomValue();
            string table = "<div class='panel panel-default dt-panel'>";
            table += "<div class='panel-heading'>" + this.title + "</div>";
            table += "<table class='table table-striped table-bordered table' id='" + tableID + "'>";

            //create table headers
            table += "<thead>";
            table += "<tr>";
            foreach (var header in headers)
            {
                table += "<th>" + header.GetType().GetProperty("Name").GetValue(header).ToString() + "</th>";
            }
            //add actions column and close header row and table header
            table += "<th>Actions</th>";
            table += "</tr>";
            table += "</thead>";

            //open the table body for data display
            table += "<tbody>";

            foreach (var item in this.data)
            {
                table += "<tr>";
                foreach (var header in headers)
                {
                    string property = header.GetType().GetProperty("Field").GetValue(header).ToString();
                    table += "<td>" + item.GetType().GetProperty(property).GetValue(item) + "</td>";
                }
                table += "<td>";
                foreach (var action in actions)
                {
                    string id = item.GetType().GetProperty("id").GetValue(item).ToString();
                    string text = action.GetType().GetProperty("text").GetValue(action).ToString();
                    string url = action.GetType().GetProperty("url").GetValue(action).ToString().Replace("{{id}}", id);
                    table += "<a href='" + url + "' class='btn btn-primary btn-xs'>" + text + "</a>&nbsp&nbsp";
                }
                table += "</td>";

                table += "</tr>";
            }

            table += "</tbody>";

            //create a table button if there is one
            if (tableButtons.Count != 0)
            {
                table += "<tfoot><tr><td colspan='" + (headers.Count + 1) + "' class='text-center dt-no-border'>";
                foreach (var button in tableButtons)
                {
                    string text = button.GetType().GetProperty("text").GetValue(button).ToString();
                    string url = button.GetType().GetProperty("url").GetValue(button).ToString();

                    table += "<a href='" + url + "' class='btn btn-primary btn-xs'>" + text + "</a>";
                }
                table += "</td></tr></tfoot>";
            }


            table += "</table>";

            //check to see if the user wants to use the jQuery DataTable API
            if (this.searchSort)
            {
                table += "<script type='text/javascript'>";

                table += "$('#" + tableID + "').DataTable();";

                table += "</script>";
            }
            table += "</div>";

            return new HtmlString(table);
        }

        private string randomValue()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
        }
    }
}