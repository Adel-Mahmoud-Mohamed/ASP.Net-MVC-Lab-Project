using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITIEntities.Model
{
    public class Student
    {
        // The EF will detect that this is the pk for the student table because of the name (Id) and type (int)
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        [Required]
        public string Email { get; set; }

        [ForeignKey(nameof(Department))]
        public int Deptno { get; set; }

        public virtual Department Department { get; set; } = new Department();

        public virtual List<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Age: {Age}, Email: {Email}, Deptno: {Deptno}";
        }

    }
}
