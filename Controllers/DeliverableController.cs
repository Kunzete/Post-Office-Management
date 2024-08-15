using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;
using Post_Office_Management.Models;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin, employee")]
    [Route("Dashboard/[controller]/[action]")]
    public class DeliverableController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DeliverableController> _logger;


        public DeliverableController(ApplicationDbContext db, ILogger<DeliverableController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult List()
        {
            return View(_db.Deliverables.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
