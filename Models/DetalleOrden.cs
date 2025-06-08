using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Canela.Models
{
    [Table("t_order_detail")]
    public class DetalleOrden
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }
        public Decimal Precio { get; set; }
        public Orden? Orden { get; set; }
    }
}