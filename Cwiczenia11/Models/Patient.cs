using System.ComponentModel.DataAnnotations;

namespace CW10.Models;

public class Patient
{
    [Key]
    public int IdPatient { set; get; }
    [MaxLength(100)]
    public string FirstName { set; get; } = string.Empty;
    [MaxLength(100)]
    public string LastName { set; get; } = string.Empty;
    public DateTime Birthdate { set; get; }

    public ICollection<Prescription> Prescriptions { get; set; } = new HashSet<Prescription>();
}
