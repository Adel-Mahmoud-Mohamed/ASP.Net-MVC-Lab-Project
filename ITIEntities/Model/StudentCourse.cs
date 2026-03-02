using System.ComponentModel.DataAnnotations.Schema;

namespace ITIEntities.Model
{
    public class StudentCourse
    {
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        
        [ForeignKey(nameof(Course))]
        public int CrsNo { get; set; }

        public int? Degree { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}