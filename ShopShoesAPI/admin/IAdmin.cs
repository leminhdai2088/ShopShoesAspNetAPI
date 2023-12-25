using ShopShoesAPI.common;
using ShopShoesAPI.user;

namespace ShopShoesAPI.admin
{
    public interface IAdmin
    {
        // user
        public Task<List<UserDTO>> FindAllUser(QueryAndPaginateDTO queryAndPaginate);

        public Task<Summary> SummaryStats();
        public Task<string> DeleteUser(string id);
    }
}
