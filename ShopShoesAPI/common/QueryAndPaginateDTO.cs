using System.Diagnostics.CodeAnalysis;

namespace ShopShoesAPI.common
{
    public class QueryAndPaginateDTO
    {
        public int pageIndex { get; set; } = 1;

        public int pageSize { get; set; } = 10;

        public string searchString { get; set; } = string.Empty;

        public string sortBy { get; set; } = string.Empty;

    }
}
