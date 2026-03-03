using System;
using System.Linq;
using ITIEntities.Data;
using ITIEntities.Model;
using ITIEntities.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        IEntityRepo<Course> courseRepo;
        ITIContext db;

        public CourseController(IEntityRepo<Course> courseRepo, ITIContext db)
        {
            this.courseRepo = courseRepo;
            this.db = db;
        }

        public IActionResult Index()
        {
            var model = courseRepo.GetAll();
            return View(model);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var model = courseRepo.GetById(id.Value);
            if (model == null) return NotFound();
            return View(model);
        }

        public IActionResult Create()
        {
            // departments to allow assigning course to departments if needed
            ViewBag.Departments = db.Departments.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course, int[] departments)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = db.Departments.ToList();
                return View(course);
            }
            
            try
            {
                // attach selected departments
                if (departments != null && departments.Length > 0)
                {
                    course.Departments = db.Departments.Where(d => departments.Contains(d.DeptId)).ToList();
                }

                courseRepo.Add(course);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the course: " + ex.Message);
                ViewBag.Departments = db.Departments.ToList();
                return View(course);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var model = courseRepo.GetById(id.Value);
            if (model == null) return NotFound();
            ViewBag.Departments = db.Departments.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course edited, int[] departments)
        {
            if (id != edited.CrsId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = db.Departments.ToList();
                return View(edited);
            }

            try
            {
                // Load the course using the same DbContext (db) so all tracked entities are from one context
                var existing = db.Courses.Include(c => c.Departments).FirstOrDefault(c => c.CrsId == id);
                if (existing == null) return NotFound();

                existing.Name = edited.Name;
                existing.Duration = edited.Duration;

                // update departments association using entities from the same context
                existing.Departments.Clear();
                if (departments != null && departments.Length > 0)
                {
                    var deps = db.Departments.Where(d => departments.Contains(d.DeptId)).ToList();
                    foreach (var d in deps) existing.Departments.Add(d);
                }

                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the course: " + ex.Message);
                ViewBag.Departments = db.Departments.ToList();
                return View(edited);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var model = courseRepo.GetById(id.Value);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var existing = courseRepo.GetById(id);
                if (existing == null) return NotFound();
                courseRepo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the course: " + ex.Message);
                var model = courseRepo.GetById(id);
                return View(model);
            }
        }

        // GET: Course/ManageGrades/5
        [Authorize(Roles = "admin")]
        public IActionResult ManageGrades(int? id)
        {
            if (id == null) return NotFound();
            var course = db.Courses
                           .Include(c => c.CourseStudents)
                           .ThenInclude(sc => sc.Student)
                           .FirstOrDefault(c => c.CrsId == id.Value);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Course/ManageGrades/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult ManageGrades(int id)
        {
            var course = db.Courses
                           .Include(c => c.CourseStudents)
                           .FirstOrDefault(c => c.CrsId == id);
            if (course == null) return NotFound();

            try
            {
                foreach (var sc in course.CourseStudents)
                {
                    var formKey = $"degree_{sc.StudentId}";
                    var value = Request.Form[formKey].FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        sc.Degree = null;
                    }
                    else if (int.TryParse(value, out var parsed))
                    {
                        sc.Degree = parsed;
                    }
                }

                db.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving degrees: " + ex.Message);
                // reload students for the view
                course = db.Courses
                           .Include(c => c.CourseStudents)
                           .ThenInclude(sc => sc.Student)
                           .FirstOrDefault(c => c.CrsId == id);
                return View(course);
            }
        }
    }
}
