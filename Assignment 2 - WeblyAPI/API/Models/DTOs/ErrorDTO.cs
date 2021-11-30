using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTOs
{
    public class ErrorDTO
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
    }
}