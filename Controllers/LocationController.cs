using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;
using Post_Office_Management.Models;

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

        [HttpPost]
        public async Task<IActionResult> Create(Office location)
        {
            if (ModelState.IsValid)
            {
                _db.Locations.Add(location);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(location);
        }
    }
}
