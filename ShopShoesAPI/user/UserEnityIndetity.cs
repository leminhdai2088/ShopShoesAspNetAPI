using Microsoft.AspNetCore.Identity;

namespace ShopShoesAPI.user
{
    public class UserEnityIndetity: IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
