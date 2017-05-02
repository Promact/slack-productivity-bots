using System.Web.Optimization;

namespace Promact.Erp.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/font-lineicons.css",
                "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/bundles/afterlogin").Include(
               "~/Content/css/font-awesome.min.css",
               "~/Content/css/custom.min.css",
               "~/Content/css/erp-custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                 "~/Content/js/icheck.js",
                "~/Content/js/custom.js"));
        }
    }
}
