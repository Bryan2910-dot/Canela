using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Canela.Models
{
    [Table("t_producto")]
    public class Producto
    {
        [Key]
        [Column("Id")] // Exactamente como en tu BD
        public int Id { get; set; }

        [Column("Name")] 
        [Required]
        public string Name { get; set; }

        [Column("Price")] // Nombre exacto de la columna
        [Required]
        public string PriceText { get; set; }

        [NotMapped] // Propiedad calculada
        public decimal Price => decimal.TryParse(PriceText, out var result) ? result : 0;

        [Column("Status")] // Primera letra mayúscula
        [Required]
        public string Status { get; set; }

        [Column("ImageURL")]
        [Required]
        public string ImageURL { get; set; }
    }
}