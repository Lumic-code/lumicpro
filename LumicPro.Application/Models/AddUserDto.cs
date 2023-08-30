using LumicPro.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LumicPro.Application.Models
{
    public class AddUserDto
    {
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