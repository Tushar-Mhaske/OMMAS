using System.Web;
using System.Web.Optimization;

namespace PMGSY
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=25
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.migrate.js"
                        ));                       

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                      // "~/Scripts/jquery.unobtrusive-ajax.min",
                        "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.js"
                        
                        
                        ));

            bundles.Add(new ScriptBundle("~/bundles/menubar").Include(
                        "~/Scripts/jquery.ui.core.js",
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jquery.ui.position.js",
                        "~/Scripts/jquery.ui.menu.js",
                        "~/Scripts/jquery.ui.button.js",
                        "~/Scripts/menubar.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
                        "~/Scripts/i18n/grid.locale-en.js",
                        "~/Scripts/i18n/jquery.jqGrid.src.js"
                             
                        ));


            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/Scripts/jquery.themeswitcher.js",
                        "~/Scripts/jquery.qtip.js",
                        "~/Scripts/custom/GenericUIControl.js",
                        "~/Scripts/jquery.address-1.6.min.js"
                        ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //for block and unblock ui added by koustubh  nakate

            bundles.Add(new ScriptBundle("~/bundles/jqueryBlockUI").Include(
                        "~/Scripts/jquery.blockUI.min.js"));
                        

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css",
                        "~/Content/menu.css",
                        "~/Content/jquery.qtip.css",
                        "~/Content/ui.jqgrid.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include( 
                    "~/Scripts/grid.locale-en.js",
                    "~/Scripts/jquery.jqGrid.min.js",
                    "~/Scripts/jquery.jqGrid.src.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/themes/dark-hive").Include(
                        "~/Content/themes/dark-hive/ui.jqgrid.css",
                        "~/Content/themes/dark-hive/jquery-ui-1.10.0.custom.css"));

            //redmond theme 
            bundles.Add(new StyleBundle("~/Content/themes/redmond").Include(
                        "~/Content/themes/redmond/ui.jqgrid.css",
                        "~/Content/themes/redmond/jquery-ui-1.10.1.custom.css"

                        ));

            //flick theme 
            bundles.Add(new StyleBundle("~/Content/themes/flick").Include(
                        "~/Content/themes/flick/ui.jqgrid.css",
                        "~/Content/themes/flick/jquery-ui-1.10.1.custom.css"

                        ));

            //smothness theme 
            bundles.Add(new StyleBundle("~/Content/themes/pmgsy").Include(
                 "~/Content/themes/pmgsy/jquery-ui-1.9.2.custom.css",       
                "~/Content/themes/pmgsy/ui.jqgrid.css"
                       

                        ));


            bundles.Add(new ScriptBundle("~/scripts/FileUpload")
                //.Include("~/scripts/FileUpload/vendor/jquery.ui.widget.js")
                .Include("~/scripts/FileUpload/tmpl.js")
                .Include("~/scripts/FileUpload/load-image.js")
                .Include("~/scripts/FileUpload/canvas-to-blob.js")

                .Include("~/scripts/FileUpload/jquery.iframe-transport.js")
                .Include("~/scripts/FileUpload/jquery.fileupload.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-fp.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-ui.js")
                //.Include("~/scripts/FileUpload/locale.js")
                .Include("~/scripts/FileUpload/main.js")
                );

            bundles.Add(new StyleBundle("~/styles/Bootstrap")
                .Include("~/styles/Bootstrap/bootstrap.css")
                .Include("~/styles/Bootstrap/bootstrap-responsive.css")
            );


            bundles.Add(new ScriptBundle("~/scripts/Modernizr")
                .Include("~/scripts/modernizr-*")
                );


        }
    }
}