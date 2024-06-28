using System.ComponentModel.DataAnnotations;
using CW10.Models;

namespace CW10.DTOs;

public class CreatePrescriptionDTO
{
    [Required]
    public Patient Patient { get; set; } = null!;
    [Required]
    public Doctor Doctor { get; set; } = null!;
    [Required]
    public IEnumerable<CreateMedicamentDTO> Medicaments { get; set; } = new List<CreateMedicamentDTO>();
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
}

public class CreateMedicamentDTO
{
    [Required]
    public int IdMedicament { get; set; }
    [Required]
    public int? Dose { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
}
