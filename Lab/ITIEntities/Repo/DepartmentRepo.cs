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
    public class DepartmentRepo : IEntityRepo<Department>
    {
        ITIContext context = new ITIContext();
        public void Add(Department entity)
        {
            context.Departments.Add(entity);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            context.Departments.Remove(GetById(id));
            context.SaveChanges();
        }

        public List<Department> GetAll()
        {
            return context.Departments.Include(d => d.Students).Include(d=>d.Courses).ToList();
        }

        public List<Department> GetByCondition(Func<Department, bool> predicate)
        {
            return context.Departments.Where(predicate).ToList();
        }

        public Department GetById(int id)
        {
            return context.Departments
                .Include(d => d.Students)
                .Include(d => d.Courses)
                .FirstOrDefault(d => d.DeptId == id);
        }

        public void Update(Department entity)
        {
            context.Departments.Update(entity);
            context.SaveChanges();
        }
    }
}
