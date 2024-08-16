using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var deliverables = _db.Deliverables
                .Include(d => d.ServiceType)
                .Include(d => d.FromOffice)
                .Include(d => d.ToOffice)
                .ToList();
            return View(deliverables);
        }

        public IActionResult Create()
        {
            ViewBag.ServiceTypes = new SelectList(_db.Services, "Id", "Name");
            ViewBag.FromOffices = new SelectList(_db.Locations, "Id", "City");
            ViewBag.ToOffices = new SelectList(_db.Locations, "Id", "City");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                // Generate a unique delivery number
                delivery.DeliveryNumber = Guid.NewGuid().ToString();
                delivery.Status = "Posted";

                // Calculate the charge
                delivery.Charge = CalculateCharge(delivery);

                // Set the ServiceType, FromOffice, and ToOffice navigation properties
                delivery.ServiceType = _db.Services.Find(delivery.ServiceTypeId);
                delivery.FromOffice = _db.Locations.Find(delivery.FromOfficeId);
                delivery.ToOffice = _db.Locations.Find(delivery.ToOfficeId);

                _db.Deliverables.Add(delivery);
                await _db.SaveChangesAsync();

                return RedirectToAction("List");
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewBag.ServiceTypes = new SelectList(_db.Services, "Id", "Name", delivery.ServiceTypeId);
                ViewBag.FromOffices = new SelectList(_db.Locations, "Id", "City", delivery.FromOfficeId);
                ViewBag.ToOffices = new SelectList(_db.Locations, "Id", "City", delivery.ToOfficeId);
                return View(delivery);
            }
        }

        private decimal CalculateCharge(Delivery delivery)
        {
            try
            {
                // Retrieve the applicable service
                var service = _db.Services
                    .Where(s => s.Id == delivery.ServiceTypeId)
                    .FirstOrDefault();

                if (service == null)
                {
                    // Log a warning if no matching service found
                    _logger.LogWarning("No service found for delivery.");
                    return 0;
                }

                // Retrieve the applicable charge detail
                var chargeDetail = _db.Charges
                    .Where(cd => cd.ServiceTypeId == delivery.ServiceTypeId)
                    .Where(cd => GetWeightRange(delivery.Weight) == cd.WeightRange)
                    .OrderByDescending(cd => cd.EffectiveDate)
                    .FirstOrDefault();

                if (chargeDetail == null)
                {
                    // Log a warning if no matching charge detail found
                    _logger.LogWarning("No charge detail found for delivery.");
                    return 0;
                }

                // Calculate the total charge
                decimal totalCharge = service.BaseCharge + chargeDetail.Charge;

                return totalCharge;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating charge");
                return 0;
            }
        }

        private WeightRange GetWeightRange(decimal weight)
        {
            if (weight <= 100)
                return WeightRange.ZeroToOneHundredGrams;
            else if (weight <= 500)
                return WeightRange.OneHundredOneToFiveHundredGrams;
            else if (weight <= 1000)
                return WeightRange.FiveHundredOneToOneThousandGrams;
            else if (weight <= 2000)
                return WeightRange.OneThousandOneToTwoThousandGrams;
            else
                return WeightRange.TwoThousandOneAndAboveGrams;
        }
    }
}
