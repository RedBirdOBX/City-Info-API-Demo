namespace CityInfoAPI.Web.Controllers.RequestHelpers
{
    #pragma warning disable CS1591

    public class CitiesPagingParameters
    {
        private const int _minPageNumber = 1;
        private const int _minPageSize = 1;
        private const int _maxPageSize = 10;

        private int _pageNumber;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }

            set
            {
                // this will prevent consumer asking for anything less than 1.
                _pageNumber = (value < _minPageNumber) ? _minPageNumber : value;
            }
        }

        private int _pageSize;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                // this will prevent the consumer sending anything more than the max and less than the min.
                _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
                _pageSize = (_pageSize < _minPageSize) ? _minPageSize : _pageSize;
            }
        }
    }

    #pragma warning restore CS1591
}
