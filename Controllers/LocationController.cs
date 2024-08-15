using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin, employee")]
    [Route("Dashboard/[controller]/[action]")]
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LocationController> _logger;


        public LocationController(ApplicationDbContext db, ILogger<LocationController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult List()
        {
            return View(_db.Locations.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
