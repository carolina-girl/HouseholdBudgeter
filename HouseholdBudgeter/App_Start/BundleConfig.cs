using System.Web;
using System.Web.Optimization;

namespace HouseholdBudgeter
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Template/css").Include(
           "~/Template/vendor/metisMenu/metisMenu.min.css",
           "~/Template/dist/css/sb-admin-2.css",
           "~/Template/vendor/morrisjs/morris.css",
           "~/Template/vendor/font-awesome/css/font-awesome.min.css"));


            bundles.Add(new ScriptBundle("~/Template/bootstrap").Include(
          "~/Template/vendor/metisMenu/metisMenu.min.js",
          "~/Template/vendor/raphael/raphael.min.js",
          "~/Template/vendor/morrisjs/morris.min.js",
           "~/Template/dist/js/sb-admin-2.js"));
        }
    }
}
