using System.Diagnostics;
using DemoApp.Models;
using ITIEntities.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITIContext _db;

        public HomeController(ILogger<HomeController> logger, ITIContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new Models.DashboardViewModel
            {
                DepartmentsCount = _db.Departments.Count(),
                CoursesCount = _db.Courses.Count(),
                StudentsCount = _db.Students.Count(),
                RecentStudents = _db.Students.OrderByDescending(s => s.Id).Take(5).ToList(),
                RecentCourses = _db.Courses.OrderByDescending(c => c.CrsId).Take(5).ToList()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
