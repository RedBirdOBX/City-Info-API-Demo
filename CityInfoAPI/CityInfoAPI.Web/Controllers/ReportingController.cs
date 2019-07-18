using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>
    /// returns various reports
    /// </summary>
    public class ReportingController : ControllerBase
    {
        /// <summary>
        /// returns a list of cities and its count of points of interest
        /// </summary>
        /// <returns>collection of city summary dto</returns>
        public ActionResult CitySummary()
        {
            return null;
        }
    }
}