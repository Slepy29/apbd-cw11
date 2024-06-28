using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CW10.Models;

public class Prescription
{
    [Key]
    public int IdPrescription { set; get; }
    public DateTime Date { set; get; }
    public DateTime DueDate { set; get; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    [ForeignKey(nameof(IdPatient))]
    public Doctor Doctor { get; set; } = null!;
    [ForeignKey(nameof(IdDoctor))]
    public Patient Patient { get; set; } = null!;

    public ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new HashSet<PrescriptionMedication>();
}
