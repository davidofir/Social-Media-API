using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public PagedResponse(IEnumerable<T> data)
        {
            this.Data = data;
        }
    }
}