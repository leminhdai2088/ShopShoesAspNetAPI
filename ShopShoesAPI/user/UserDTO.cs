using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShopShoesAPI.user
{
    public class UserDTO
    {
        [StringLength(100), Required]
        public string FullName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
    public class UpdateUserDTO
    {
        [StringLength(100), AllowNull]
        public string? FullName { get; set; }

        [StringLength(200), AllowNull]
        public string? Address { get; set; }
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
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
