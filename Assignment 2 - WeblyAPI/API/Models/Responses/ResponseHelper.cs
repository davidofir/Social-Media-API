using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models.Responses
{
    public class ResponseHelper<T>
    {
        public static PagedResponse<T> GetPagedResponse(string url, IEnumerable<T> result, int pageNumber, int pageSize, int totalRecords)
        {
            var response = new PagedResponse<T>(result);
            var totalPages = Math.Ceiling(totalRecords / (double)pageSize);
            response.Meta.Add("TotalRecords", totalRecords.ToString());
            response.Meta.Add("TotalPages", totalPages.ToString());
            response.Links.Add("next", pageNumber == totalPages ? "" : $"{url}?pageNumber={pageNumber + 1}&pageSize={pageSize}");
            response.Links.Add("first", $"{url}?pageNumber=1&pageSize={pageSize}");
            response.Links.Add("last", $"{url}?pageNumber={totalPages}&pageSize={pageSize}");
            return response;
        }
    }
}