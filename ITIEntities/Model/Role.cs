using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITIEntities.Model
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
