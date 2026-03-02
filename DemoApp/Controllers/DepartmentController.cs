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
    public class DepartmentController : Controller
    {
        IEntityRepo<Department> departmentRepo;
        ITIContext db;

        public DepartmentController(IEntityRepo<Department> departmentRepo, ITIContext db)
        {
            this.departmentRepo = departmentRepo;
            this.db = db;
        }


        public IActionResult Index()
        {
            var model = departmentRepo.GetAll();
            return View(model);
        }


        public IActionResult Details(int? id)
        {
            if (id == null) 
                return NotFound();

            var model = departmentRepo.GetById(id.Value);

            if (model == null)
                return NotFound();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                departmentRepo.Add(department);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the department: " + ex.Message);
                return View(department);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var model = departmentRepo.GetById(id.Value);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Department edited)
        {
            if (id != edited.DeptId)
                return BadRequest();

            // Only update Name and Capacity explicitly
            if (!ModelState.IsValid)
            {
                return View(edited);
            }

            try
            {
                var existing = departmentRepo.GetById(id);
                if (existing == null)
                    return NotFound();

                existing.DeptName = edited.DeptName;
                existing.Capacity = edited.Capacity;

                departmentRepo.Update(existing);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the department: " + ex.Message);
                return View(edited);
            }
        }

        // GET: Department/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var model = departmentRepo.GetById(id.Value);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var existing = departmentRepo.GetById(id);
                if (existing == null)
                    return NotFound();

                departmentRepo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the department: " + ex.Message);
                var model = departmentRepo.GetById(id);
                return View(model);
            }
        }

        // GET: Department/ManageDeptCourse/5
        public IActionResult ManageDeptCourse(int? id)
        {
            if (id == null)
                return NotFound();

            // Use the same DbContext instance so tracked entities are consistent
            var model = db.Departments
                          .Include(d => d.Courses)
                          .Include(d => d.Students)
                          .FirstOrDefault(d => d.DeptId == id.Value);

            if (model == null)
                return NotFound();

            var allCourses = db.Courses.ToList();
            var coursesNotInDept = allCourses.Where(c => !model.Courses.Any(mc => mc.CrsId == c.CrsId)).ToList();

            ViewBag.CoursesNotInDept = coursesNotInDept;

            return View(model);
        }

        // POST: Department/ManageDeptCourse/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageDeptCourse(int id, int[] coursestoadd, int[] coursestoremove)
        {
            var model = db.Departments.Include(d => d.Courses).FirstOrDefault(d => d.DeptId == id);
            if (model == null)
                return NotFound();

            try
            {
                // Remove selected courses
                if (coursestoremove != null && coursestoremove.Length > 0)
                {
                    foreach (var cid in coursestoremove)
                    {
                        var courseToRemove = model.Courses.FirstOrDefault(c => c.CrsId == cid);
                        if (courseToRemove != null)
                        {
                            model.Courses.Remove(courseToRemove);
                        }
                    }
                }

                // Add selected courses
                if (coursestoadd != null && coursestoadd.Length > 0)
                {
                    var coursesToAdd = db.Courses.Where(c => coursestoadd.Contains(c.CrsId)).ToList();
                    foreach (var c in coursesToAdd)
                    {
                        if (!model.Courses.Any(mc => mc.CrsId == c.CrsId))
                        {
                            model.Courses.Add(c);
                        }
                    }
                }

                db.SaveChanges();

                return RedirectToAction(nameof(ManageDeptCourse), new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating courses: " + ex.Message);
                var allCourses = db.Courses.ToList();
                ViewBag.CoursesNotInDept = allCourses.Where(c => !model.Courses.Any(mc => mc.CrsId == c.CrsId)).ToList();
                return View(model);
            }
        }
    }
}
