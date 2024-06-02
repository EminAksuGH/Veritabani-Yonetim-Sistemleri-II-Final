using System.Web.Mvc;
using System.Web.Routing;

namespace FilmKurdu
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ChangePassword",
                url: "Home/ChangePassword",
                defaults: new { controller = "Home", action = "ChangePassword" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
