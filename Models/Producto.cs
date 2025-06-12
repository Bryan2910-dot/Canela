using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Canela.Models
{
    [Table("t_producto")] // Asegúrate que coincida con tu tabla en PostgreSQL
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")] // Mapea con la columna "ID" en tu BD
        public int Id { get; set; }

        [Column("Name")] // Mapea exactamente con tu columna en PostgreSQL
        [NotNull]
        public string? Name { get; set; }

        [Column("price", TypeName = "text")] // "price" en minúscula para coincidir con tu BD
        [NotNull]
        public string? PriceText { get; set; }

        [NotMapped] // No se mapea a la BD
        public decimal Price
        {
            get => decimal.TryParse(PriceText, out var result) ? result : 0;
            set => PriceText = value.ToString();
        }

        [Column("status")] // "status" en minúscula para coincidir con tu BD
        [NotNull]
        public string? Status { get; set; }

        [Column("imageURL")]
        [NotNull]
        public string? ImageURL { get; set; }
    }
}