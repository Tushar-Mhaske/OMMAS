using System.Web.Mvc;

namespace PMGSY.Areas.BalanceHab
{
    public class BalanceHabAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BalanceHab";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BalanceHab_default",
                "BalanceHab/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
