using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Models.DTOs;

namespace API.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The name field is required")]
        public string Name { get; set; }
        public List<Image> Images { get; set; }
    }
}