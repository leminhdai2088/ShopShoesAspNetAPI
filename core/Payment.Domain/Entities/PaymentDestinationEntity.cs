using Payment.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("PaymentDestination")]
    public class PaymentDestinationEntity: BaseAuditableEntity
    {
        [Key]
        public string Id { get; set; }
        public string? DesName { get; set; } = string.Empty;
        public string? DesShortName { get; set; } = string.Empty;
        public string? DesLogo { get; set; } = string.Empty;
        public int ShortIndex { get; set; }
        public bool IsActive { get; set; }

        public string? DesParentId { get; set; } = string.Empty; //fk
        public PaymentDestinationEntity? PaymentDestinationEntities { get; set; }

        public List<PaymentEntity>? PaymentEntities { get; set; }


    }
}
