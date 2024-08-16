using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;
using Post_Office_Management.Models;

namespace Post_Office_Management.Controllers
{
    [Authorize]
    [Route("Dashboard/[controller]/[action]")]
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TrackingController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Track (string TrackingID)
        {
            var deliverable = _db.Deliverables.FirstOrDefault(d => d.DeliveryNumber== TrackingID);
            if (deliverable == null)
            {
                ViewBag.ErrorMessage = "Parcel not found.";
                return View("Index");
            }
            return View("TrackingInfo", deliverable);
        }
    }
}
