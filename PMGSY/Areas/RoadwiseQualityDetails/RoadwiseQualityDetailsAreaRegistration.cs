using System.Web.Mvc;

namespace PMGSY.Areas.RoadwiseQualityDetails
{
    public class RoadwiseQualityDetailsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RoadwiseQualityDetails";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RoadwiseQualityDetails_default",
                "RoadwiseQualityDetails/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
