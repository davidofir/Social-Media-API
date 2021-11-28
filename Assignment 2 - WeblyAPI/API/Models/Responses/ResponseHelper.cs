using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Models.Responses
{
    public static class ResponseHelper<T>
    {
        public static PagedResponse<T> GetPagedResponse
            (string url, IEnumerable<T> result, int pageNumber, int pageSize, int totalRecords)
        {
            var response = new PagedResponse<T>(result);
            var totalpages = Math.Ceiling(totalRecords / (double)pageSize);
            response.Meta.Add("totalRecords", totalRecords.ToString());
            response.Meta.Add("TotalPages", totalpages.ToString());

            response.Links.Add("next", pageNumber == totalpages ? "" : $"{url}?pageNumber={pageNumber + 1}&pagesize={pageSize}");
            response.Links.Add("previous", pageNumber == 1 ? "" : $"{url}?pageNumber={pageNumber - 1}&pagesize={pageSize}");
            response.Links.Add("first", $"{url}?pageNumber=1&pagesize={pageSize}");
            response.Links.Add("last", $"{url}?pageNumber={totalpages}&pagesize={pageSize}");
            return response;

        }
    }
}