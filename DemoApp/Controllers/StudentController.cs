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

        // GET: Student/ManageEnrollments/5
        public IActionResult ManageEnrollments(int? id)
        {
            if (id == null) return NotFound();

            var student = db.Students
                            .Include(s => s.StudentCourses).ThenInclude(sc => sc.Course)
                            .Include(s => s.Department).ThenInclude(d => d.Courses)
                            .FirstOrDefault(s => s.Id == id.Value);
            if (student == null) return NotFound();

            // courses offered by student's department
            var deptCourses = student.Department?.Courses ?? new System.Collections.Generic.List<Course>();

            // courses student not enrolled in (but in department)
            var enrolledIds = student.StudentCourses.Select(sc => sc.CrsNo).ToList();
            var available = deptCourses.Where(c => !enrolledIds.Contains(c.CrsId)).ToList();

            ViewBag.AvailableCourses = available;

            return View(student);
        }

        // POST: Student/ManageEnrollments/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageEnrollments(int id, int[] coursestoadd, int[] coursestoremove)
        {
            var student = db.Students.Include(s => s.StudentCourses).FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();

            try
            {
                // Remove selected enrollments
                if (coursestoremove != null && coursestoremove.Length > 0)
                {
                    foreach (var cid in coursestoremove)
                    {
                        var sc = student.StudentCourses.FirstOrDefault(x => x.CrsNo == cid && x.StudentId == id);
                        if (sc != null)
                        {
                            db.StudentCourses.Remove(sc);
                        }
                    }
                }

                // Add selected enrollments (only if course belongs to student's department)
                if (coursestoadd != null && coursestoadd.Length > 0)
                {
                    // load department course ids
                    var deptCourseIds = db.Departments.Include(d => d.Courses).Where(d => d.DeptId == student.Deptno).SelectMany(d => d.Courses).Select(c => c.CrsId).ToList();

                    foreach (var cid in coursestoadd.Distinct())
                    {
                        if (!student.StudentCourses.Any(x => x.CrsNo == cid) && deptCourseIds.Contains(cid))
                        {
                            db.StudentCourses.Add(new StudentCourse { StudentId = id, CrsNo = cid, Degree = null });
                        }
                    }
                }

                db.SaveChanges();

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating enrollments: " + ex.Message);
                // reload available courses and student
                student = db.Students
                            .Include(s => s.StudentCourses).ThenInclude(sc => sc.Course)
                            .Include(s => s.Department).ThenInclude(d => d.Courses)
                            .FirstOrDefault(s => s.Id == id);
                var deptCourses = student.Department?.Courses ?? new System.Collections.Generic.List<Course>();
                var enrolledIds = student.StudentCourses.Select(sc => sc.CrsNo).ToList();
                ViewBag.AvailableCourses = deptCourses.Where(c => !enrolledIds.Contains(c.CrsId)).ToList();
                return View(student);
            }
        }
    }
}
