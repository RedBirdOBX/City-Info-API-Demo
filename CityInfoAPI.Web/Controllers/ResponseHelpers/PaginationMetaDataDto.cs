namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    #pragma warning disable CS1591

    public class PaginationMetaDataDto
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public string PreviousPageUrl { get; set; }

        public string NextPageUrl { get; set; }

        public string OrderNameBy { get; set; }
    }

    #pragma warning restore CS1591
}
