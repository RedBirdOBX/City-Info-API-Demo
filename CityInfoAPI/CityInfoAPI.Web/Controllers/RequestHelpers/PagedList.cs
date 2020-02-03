using System;
using System.Collections.Generic;
using System.Linq;

namespace CityInfoAPI.Web.Controllers.RequestHelpers
{

    #pragma warning disable CS1591

    // we're going to add a little bit of additional functionality to a List<T>.

    public class PagedList<T>: List<T>
    {
        // private setter so that nothing can manipulate this val.
        public int CurrentPage { get; private set; }

        // private setter so that nothing can manipulate this val.
        public int TotalPages { get; private set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return CurrentPage > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return CurrentPage < TotalPages;
            }
        }

        // constructor
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<T> items = source.Skip((pageNumber - 1) + pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }

    #pragma warning restore CS1591

}
