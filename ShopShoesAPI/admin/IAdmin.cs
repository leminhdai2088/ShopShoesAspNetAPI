using ShopShoesAPI.common;
using ShopShoesAPI.enums;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;

namespace ShopShoesAPI.admin
{
    public interface IAdmin
    {
        // user
        public Task<List<UserDTO>> FindAllUser(QueryAndPaginateDTO queryAndPaginate, StatusUserEnum? statusUserEnum); 
        public Task<bool> DeleteUserById(string id);

        // order
        public Task<IEnumerable<object>> FindAllOrder(QueryAndPaginateDTO queryAndPaginate, OrderStatusEnum? status);


    }
}
