using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.order;

namespace ShopShoesAPI.user
{
    public class UserEnityIndetity: IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;

        public ICollection<OrderEntity> Orders { get; set; }
    }
}
