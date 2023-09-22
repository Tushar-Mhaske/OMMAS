namespace $rootnamespace$.Areas.Admin 
{
    using System;
    using System.Web.Mvc;

    // ReSharper disable UnusedMember.Global
    public class AdminAreaRegistration : AreaRegistration
    // ReSharper restore UnusedMember.Global
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
