using ITIEntities.Model;
using System.Collections.Generic;

namespace DemoApp.Models
{
    public class DashboardViewModel
    {
        public int DepartmentsCount { get; set; }
        public int CoursesCount { get; set; }
        public int StudentsCount { get; set; }

        public List<Student> RecentStudents { get; set; } = new List<Student>();
        public List<Course> RecentCourses { get; set; } = new List<Course>();
    }
}
