namespace $rootnamespace$.Areas.Admin.Controllers 
{
    using System.Web.Mvc;

    ////[Authorize(Roles = "Admin")]
    public class ElmahController : Controller
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult Index()
        {
            return new ElmahResult();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult StyleSheet()
        {
            return new ElmahResult("stylesheet");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSS", Justification = "RSS is an abbreviation")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult RSS()
        {
            return new ElmahResult("rss");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "RSS", Justification = "RSS is an abbreviation")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult DigestRSS()
        {
            return new ElmahResult("digestrss");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult About()
        {
            return new ElmahResult("about");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult Detail()
        {
            return new ElmahResult("detail");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult Download()
        {
            return new ElmahResult("download");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult Json()
        {
            return new ElmahResult("json");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "CodeAnalysis unaware of ASP.NET MVC")]
        public ActionResult Xml()
        {
            return new ElmahResult("xml");
        }
    }
}
