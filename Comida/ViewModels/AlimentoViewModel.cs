using System.ComponentModel.DataAnnotations;

namespace Comida.ViewModels;

public class IngredienteViewModel
{
    [Required]
    [StringLength(40)]
    public string Nombre { get; set; }

    [Required]
    public int Calorias { get; set; }
}