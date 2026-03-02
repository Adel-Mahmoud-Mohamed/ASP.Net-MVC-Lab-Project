using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITIEntities.Model
{
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeptId { get; set; }

        [Required, MaxLength(50)]
        public string DeptName { get; set; }

        public int Capacity { get; set; }

        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();

        public virtual List<Course> Courses { get; set; } = new List<Course>();
    }
}
