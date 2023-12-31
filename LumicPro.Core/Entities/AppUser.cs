﻿using LumicPro.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LumicPro.Core.Entities
{
    public class AppUser : IdentityUser
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