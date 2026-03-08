using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITIEntities.Model
{
    public class Course
    {
        public int CrsId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }

        // soft-delete flag to preserve historical StudentCourse records
        // TODO: make a base entity with with IsDeleted for all entities to inherit from and apply the query filter in the base entity configuration
        public bool IsDeleted { get; set; } = false;

        public virtual List<Department> Departments { get; set; } = new List<Department>();
        public virtual List<StudentCourse> CourseStudents { get; set; } = new List<StudentCourse>();
    }
}
