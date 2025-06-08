using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Canela.Models
{
    [Table("t_pago")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal MontoTotal { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? Status { get; set; }

        // Relación con el usuario (AspNetUsers)
        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }

        // Relación con la orden
        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public Orden? Order { get; set; }

        // Opcional: campos para PayPal
        public string? PayPalPaymentId { get; set; }
        public string? PayPalPayerId { get; set; }
    }
}