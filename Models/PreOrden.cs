using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Canela.Models
{
    [Table("t_preorden")]
    public class PreOrden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }
        
        public string? UserName { get; set; }
        
        public Producto? Producto { get; set; }
        
        public int Cantidad { get; set; }

        // Columna real en la base de datos (texto)
        [Column("Precio", TypeName = "text")]  // Asegúrate de que coincida con el nombre en PostgreSQL
        [NotNull]
        public string? PrecioText { get; set; }

        // Propiedad no mapeada para trabajar con el valor numérico
        [NotMapped]
        public decimal Precio  // Usamos decimal para manejar valores monetarios
        { 
            get => decimal.TryParse(PrecioText, out var result) ? result : 0m;
            set => PrecioText = value.ToString(CultureInfo.InvariantCulture); // Evita problemas de formato
        }
        
        public string UserId { get; set; }
    }
}