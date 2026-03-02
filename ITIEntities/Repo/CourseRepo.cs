using ITIEntities.Data;
using ITIEntities.Model;
using ITIEntities.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITIEntities.Repo
{
    public class CourseRepo : IEntityRepo<Course>
    {
        ITIContext context = new ITIContext();

        public void Add(Course entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // If departments were provided from another context, replace them with tracked instances from this context
            if (entity.Departments != null && entity.Departments.Any())
            {
                var ids = entity.Departments.Select(d => d.DeptId).ToList();
                var tracked = context.Departments.Where(d => ids.Contains(d.DeptId)).ToList();
                entity.Departments = tracked;
            }

            context.Courses.Add(entity);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            context.Courses.Remove(GetById(id));
            context.SaveChanges();
        }

        public List<Course> GetAll()
        {
            return context.Courses.Include(c => c.Departments).Include(c => c.CourseStudents).ToList();
        }

        public Course GetById(int id)
        {
            return context.Courses
                .Include(c => c.Departments)
                .Include(c => c.CourseStudents)
                .ThenInclude(sc => sc.Student)
                .FirstOrDefault(c => c.CrsId == id);
        }

        public List<Course> GetByCondition(Func<Course, bool> predicate)
        {
            return context.Courses.Include(c => c.Departments).Where(predicate).ToList();
        }

        public void Update(Course entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = context.Courses.Include(c => c.Departments).FirstOrDefault(c => c.CrsId == entity.CrsId);
            if (existing != null)
            {
                existing.Name = entity.Name;
                existing.Duration = entity.Duration;

                // Update departments using tracked instances from this context
                existing.Departments.Clear();
                if (entity.Departments != null && entity.Departments.Any())
                {
                    var ids = entity.Departments.Select(d => d.DeptId).ToList();
                    var tracked = context.Departments.Where(d => ids.Contains(d.DeptId)).ToList();
                    foreach (var d in tracked) existing.Departments.Add(d);
                }

                context.SaveChanges();
                return;
            }

            // fallback
            context.Courses.Update(entity);
            context.SaveChanges();
        }
    }
}
