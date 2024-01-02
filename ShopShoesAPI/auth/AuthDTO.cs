using System.ComponentModel.DataAnnotations;

namespace ShopShoesAPI.auth
{
    public class RegisterDTO
    {
        [StringLength(100), Required]
        public string FullName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }

    public class LoginDTO
    {
        [StringLength(100), Required, EmailAddress]
        public string Email { get; set; }

        [MinLength(6), MaxLength(20), Required]
        public string Password { get; set; }
    }

    public class TokenDTO
    {
        public string Id { get; set; }
        public IList<string> Role { get; set; }
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

  
}
