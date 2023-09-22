using System.Web.Mvc;

namespace PMGSY.Areas.PackageAgreementSanctionList
{
    public class PackageAgreementSanctionListAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PackageAgreementSanctionList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PackageAgreementSanctionList_default",
                "PackageAgreementSanctionList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
