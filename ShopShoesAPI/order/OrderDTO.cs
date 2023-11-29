using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShopShoesAPI.order
{
    public class OrderDTO
    {
        [Phone, AllowNull]
        public string Phone { get; set; }

        [StringLength(200), AllowNull]
        public string Address { get; set; }

        [StringLength(100), AllowNull]
        public string Note { get; set; }
    }
}
