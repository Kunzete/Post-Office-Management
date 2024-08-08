using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using SQLitePCL;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("Employees/[action]")]

    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(ILogger<EmployeeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult List()
        {
            return View(_db.Employees.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Name, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the "Employee" role to the newly registered user
                    var role = await _roleManager.FindByNameAsync("employee");
                    if (role == null)
                    {
                        // Create the "Employee" role if it doesn't exist
                        role = new IdentityRole("employee");
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddToRoleAsync(user, role.Name);

                    // Save the employee data to the database
                    var emp = new Employee  
                    {
                        LoginIdentity = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        CreatedBy = user.Id,
                        CreatedAt = DateTime.Now
                    };
                    _db.Employees.Add(emp);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("List");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (employee == null || user == null)
            {
                return NotFound("Employee or user not found");
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee model)
        {
            // Update the employee and user data
            var employee = _db.Employees.Find(model.EmployeeID);
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (employee == null || user == null)
            {
                return NotFound("Employee or user not found");
            }

            if (ModelState.IsValid)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.PhoneNumber = model.PhoneNumber;
                employee.Address = model.Address;
                employee.UpdatedAt = DateTime.Now;
                employee.UpdatedBy = "Admin";
                if(model.Password != null)
                {
                    employee.Password = model.Password;
                }

                // Update the user data
                user.UserName = model.Name;
                user.Email = model.Email;

                if (!string.IsNullOrEmpty(Request.Form["OldPassword"]) && !string.IsNullOrEmpty(Request.Form["Password"]))
                {
                    var passwordResult = await _userManager.ChangePasswordAsync(user, Request.Form["OldPassword"], Request.Form["Password"]);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(employee);
                    }
                }

                await _userManager.UpdateAsync(user);
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _db.Employees.Find(id);

            if ( employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Employee employee)
        {
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            _logger.LogInformation($"Email: {employee.Email}");

            var user = await _userManager.FindByEmailAsync(employee.Email);

            _db.Employees.Remove(employee);
            _db.SaveChanges();
            await _userManager.DeleteAsync(user);
            return RedirectToAction("List");
        }
    }
}
