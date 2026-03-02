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
    public class StudentController : Controller
    {
        IEntityRepo<Student> studentRepo;
        ITIContext db;

        public StudentController(IEntityRepo<Student> studentRepo, ITIContext db)
        {
            this.studentRepo = studentRepo;
            this.db = db;
        }


        public IActionResult Index()
        {
            return View(studentRepo.GetAll());
        }

        public IActionResult Create()
        {
            // populate departments for the dropdown
            ViewBag.Departments = db.Departments.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Departments = db.Departments.ToList();
            //    return View(student);
            //}
            try
            {
                studentRepo.Add(student);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the student: " + ex.Message);
                ViewBag.Departments = db.Departments.ToList();
                return View(student);
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var model = db.Students.Include(s => s.Department).Include(s => s.StudentCourses).ThenInclude(sc => sc.Course).FirstOrDefault(s => s.Id == id.Value);
            if (model == null) return NotFound();

            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = studentRepo.GetById(id.Value);
            if (model == null) return NotFound();

            ViewBag.Departments = db.Departments.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student edited)
        {
            if (id != edited.Id) return BadRequest();

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Departments = db.Departments.ToList();
            //    return View(edited);
            //}

            try
            {
                var existing = studentRepo.GetById(id);
                if (existing == null) return NotFound();

                existing.Name = edited.Name;
                existing.Age = edited.Age;
                existing.Email = edited.Email;
                existing.Deptno = edited.Deptno;

                studentRepo.Update(existing);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the student: " + ex.Message);
                ViewBag.Departments = db.Departments.ToList();
                return View(edited);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = db.Students.Include(s => s.Department).Include(s => s.StudentCourses).ThenInclude(sc => sc.Course).FirstOrDefault(s => s.Id == id.Value);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var existing = studentRepo.GetById(id);
                if (existing == null) return NotFound();

                studentRepo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the student: " + ex.Message);
                var model = db.Students.Include(s => s.Department).Include(s => s.StudentCourses).ThenInclude(sc => sc.Course).FirstOrDefault(s => s.Id == id);
                return View(model);
            }
        }
    }
}
