using System.ComponentModel.DataAnnotations;

namespace ShopShoesAPI.auth
{
    public class RegisterDTO
    {
        [StringLength(100), Required]
        public string FullName { get; set; }

        [StringLength(100), Required]
        public string Email { get; set; }

        [MinLength(6), MaxLength(20), Required]
        public string Password { get; set; }

        [StringLength(10), Required]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }

    public class LoginDTO
    {
        [StringLength(100), Required]
        public string Email { get; set; }

        [MinLength(6), MaxLength(20), Required]
        public string Password { get; set; }
    }

    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

  
}
