using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DTOs
{
    public class AuthUserDto
    {
        [Required]
        [MaxLength(256)]
        public String Username {get; set;}

        [Required]
        [MaxLength(256)]
        public String PassWord {get; set;}
    }
}