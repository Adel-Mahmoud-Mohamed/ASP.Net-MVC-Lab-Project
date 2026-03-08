using ITIEntities.Data;
using ITIEntities.Model;
using ITIEntities.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITIEntities.Repo
{
    public class StudentRepo : IEntityRepo<Student>
    {
        ITIContext context = new ITIContext();
        public List<Student> GetAll()
        {
            return context.Students.Include(s => s.Department).ToList();
        }
        public Student GetById(int id)
        {
            return context.Students.Find(id);
        }
        public void Add(Student student)
        {
            // Prevent EF from trying to insert a Department when only Deptno is supplied
            if (student == null) throw new ArgumentNullException(nameof(student));

            // If Deptno is provided, attach the existing Department to avoid inserting a new one
            if (student.Deptno != 0)
            {
                var existingDept = context.Departments.Find(student.Deptno);
                if (existingDept != null)
                {
                    // detach any incoming Department instance and attach the tracked one
                    student.Department = existingDept;
                }
                else
                {
                    // Deptno invalid - throw so callers can handle and show a validation error
                    throw new InvalidOperationException($"Department with id {student.Deptno} does not exist.");
                }
            }
            else
            {
                // No Dept selected - ensure navigation property is null so EF won't try to insert it
                student.Department = null;
            }

            context.Students.Add(student);
            context.SaveChanges();
        }
        public void Update(Student student)
        {
            context.Students.Update(student);
            context.SaveChanges();
        }
        public void Delete(int id)
        {
            context.Students.Remove(GetById(id));
            context.SaveChanges();
        }

        public List<Student> GetByCondition(Func<Student, bool> predicate)
        {
            return context.Students.Where(predicate).ToList();
        }
    }
}
