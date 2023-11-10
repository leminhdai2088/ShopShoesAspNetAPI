using Microsoft.AspNetCore.Identity;
using ShopShoesAPI.enums;
using System.ComponentModel.DataAnnotations;

namespace ShopShoesAPI.user
{
    public class UpdateUserDTO
    {
        [StringLength(100), Required]
        public string FullName { get; set; }

        [StringLength(100), Required]
        public string Email { get; set; }

        [StringLength(10), Required]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }

    public class ChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }

        [Required, MinLength(6), MaxLength(20)]
        public string NewPassword { get; set; }
    }

    public class PayloadTokenDTO
    {
        public int Id { get; set; }
        public Roles Role { get; set; }
    }
}
