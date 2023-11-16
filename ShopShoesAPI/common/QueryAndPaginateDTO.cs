using System.Diagnostics.CodeAnalysis;

namespace ShopShoesAPI.common
{
    public class QueryAndPaginateDTO
    {
        [AllowNull]
        public int pageIndex { get; set; } = 1;
        [AllowNull]

        public int pageSize { get; set; } = 10;
        [AllowNull]

        public string searchString { get; set; } = string.Empty;
    }
}
