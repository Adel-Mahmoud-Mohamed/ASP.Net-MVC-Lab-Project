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

        public virtual List<Department> Departments { get; set; } = new List<Department>();
        public virtual List<StudentCourse> CourseStudents { get; set; } = new List<StudentCourse>();
    }
}
