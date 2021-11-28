using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTOs
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> imageUrls { get; set; }
    }
}