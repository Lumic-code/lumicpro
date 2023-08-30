using LumicPro.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LumicPro.Core.Entities
{
    public class AppUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Lenght must be 3 - 50 characters")]
        public string FirstName { get; set; }
       
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Lenght must be 3 - 50 characters")]
        public string LastName { get; set; }
       
        [Required]
        public string AttendanceStatus { get; set; }

    }
}