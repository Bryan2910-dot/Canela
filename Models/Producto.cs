using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Canela.Models
{
    [Table("t_producto")]
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [NotNull]
        public string? Name { get; set; }
        
        // Columna real en la base de datos (texto)
        [Column("Price", TypeName = "text")]
        [NotNull]
        public string? PriceText { get; set; }
        
        // Propiedad no mapeada para trabajar con el valor numérico
        [NotMapped]
        public int Price 
        { 
            get => int.TryParse(PriceText, out var result) ? result : 0;
            set => PriceText = value.ToString();
        }
        
        [NotNull]
        public string? Status { get; set; }
        
        [NotNull]
        public string? ImageURL { get; set; }
    }
}