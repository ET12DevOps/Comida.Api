using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comida.Modelo;

[Table("Plato")]
public class Plato
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string Nombre { get; set; }

    public List<Ingrediente> Ingredientes { get; set; }

    public Plato()
    {
        Ingredientes = new List<Ingrediente>();
    }
}