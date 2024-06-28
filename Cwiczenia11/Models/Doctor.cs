using System.ComponentModel.DataAnnotations;

namespace CW10.Models;

public class Doctor
{
    [Key]
    public int IdDoctor { set; get; }
    [MaxLength(100)]
    public string FirstName { set; get; } = string.Empty;
    [MaxLength(100)]
    public string LastName { set; get; } = string.Empty;
    [MaxLength(100)]
    public string Email { set; get; } = string.Empty;

    public ICollection<Prescription> Prescriptions { get; set; } = new HashSet<Prescription>();
}
