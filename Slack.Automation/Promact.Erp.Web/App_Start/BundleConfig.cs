using System.Web.Optimization;

namespace Promact.Erp.Web
{
    public static class BundleConfig
    {
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
                "~/Content/css/font-lineicons.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/afterlogin").Include(
               "~/Content/css/font-awesome.min.css",
               "~/Content/css/custom.min.css",
               "~/Content/css/erp-custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                 "~/Content/js/icheck.js",
                "~/Content/js/custom.js"));
            BundleTable.EnableOptimizations = false;
        }
    }
}
