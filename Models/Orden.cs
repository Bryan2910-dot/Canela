using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Canela.Models
{
    [Table("t_order")]
    public class Orden
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        public string? UserName { get; set; }

        public Decimal Total { get; set; }

        public DateTime Fecha { get; set; }

        public Pago? Pago { get; set; }


        public string? Status { get; set; }
    }
}