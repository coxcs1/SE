using System.Web;
using System.Web.Optimization;

namespace SoftwareEngineering1Project
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryDataTables").Include(
                        "~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                        "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            //bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        //"~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-3.4.0.js"));
						
			bundles.Add(new ScriptBundle("~/bundles/select2").Include(
						"~/Scripts/select2.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/bundles/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css",
                        "~/Content/font-awesome.css",
                        "~/Content/toastr.css",
                        "~/Content/DataTables/css/jquery.dataTables.css",
                        "~/Content/DataTables/images/details_close.png",
                        "~/Content/DataTables/images/details_open.png",
                        "~/Content/DataTables/images/sort_asc.png",
                        "~/Content/DataTables/images/sort_asc_disabled.png",
                        "~/Content/DataTables/images/sort_both.png",
                        "~/Content/DataTables/images/sort_desc.png",
                        "~/Content/DataTables/images/sort_desc_disabled.png"
                        ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
              "~/Content/themes/base/accordian.css",
              "~/Content/themes/base/all.css",
              "~/Content/themes/base/autocomplete.css",
              "~/Content/themes/base/base.css",
              "~/Content/themes/base/button.css",
              "~/Content/themes/base/core.css",
              "~/Content/themes/base/datepicker.css",
              "~/Content/themes/base/dialog.css",
              "~/Content/themes/base/draggable.css",
              "~/Content/themes/base/jquery-ui.css",
              "~/Content/themes/base/jquery-ui.min.css",
              "~/Content/themes/base/menu.css",
              "~/Content/themes/base/progressbar.css",
              "~/Content/themes/base/resizable.css",
              "~/Content/themes/base/selectable.css",
              "~/Content/themes/base/selectmenu.css",
              "~/Content/themes/base/slider.css",
              "~/Content/themes/base/sortable.css",
              "~/Content/themes/base/spinner.css",
              "~/Content/themes/base/tabs.css",
              "~/Content/themes/base/theme.css",
              "~/Content/themes/base/tooltip.css"
              ));
			
			bundles.Add(new StyleBundle("~/Content/select2").Include(
              "~/Content/css/select2.css"));
        }
    }
}
