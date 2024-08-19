using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Post_Office_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var deliverable = _db.Deliverables.Count();
            ViewBag.shipmentCount = deliverable;

            return View();
        }

        public IActionResult Parcel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Parcel([Required] string TrackingID)
        {
            if (string.IsNullOrEmpty(TrackingID))
            {
                return BadRequest("Tracking ID is required.");
            }

            var delivery = _db.Deliverables
                .Where(d => d.DeliveryNumber == TrackingID.Trim().ToLower())
                .Include(o => o.FromOffice)
                .Include(o => o.ToOffice)
                .Include(o => o.ServiceType)
                .FirstOrDefault();

            if (delivery != null)
            {
                return View(delivery);
            }
            else
            {
                ViewBag.ErrorMessage = "No delivery found with the specified tracking ID.";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
