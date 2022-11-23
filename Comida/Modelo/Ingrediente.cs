using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comida.Modelo;

[Table("Ingrediente")]
public class Ingrediente
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string Nombre { get; set; }

    [Required]
    public int Calorias { get; set; }
}