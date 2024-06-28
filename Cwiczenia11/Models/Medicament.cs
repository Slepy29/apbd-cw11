using System.ComponentModel.DataAnnotations;

namespace CW10.Models;

public class Medicament
{
    [Key]
    public int IdMedicament { set; get; }
    [MaxLength(100)]
    public string Name { set; get; } = string.Empty;
    [MaxLength(100)]
    public string Description { set; get; } = string.Empty;
    [MaxLength(100)]
    public string Type { set; get; } = string.Empty;

    public ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new HashSet<PrescriptionMedication>();
}
