using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Post_Office_Management.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TrackingController> _logger;

        public TrackingController(ApplicationDbContext db, ILogger<TrackingController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize]
        [Route("Dashboard/[controller]/[action]")]
        public IActionResult Search()
        {
            return View();
        }

        [Authorize]
        [Route("Dashboard/[controller]/[action]")]
        public IActionResult Parcel()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Dashboard/[controller]/[action]")]
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
    }
}