using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTOs
{
    public class ExpandedImageDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Guid UserId { get; set; }
        public List<string> Tags { get; set; }
    }
}