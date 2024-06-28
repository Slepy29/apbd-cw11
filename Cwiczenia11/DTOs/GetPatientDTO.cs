using CW10.Models;
namespace CW10.DTOs;

public class GetPatientDTO
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public IEnumerable<GetPrescriptionDTO> Prescriptions { get; set; } = new List<GetPrescriptionDTO>();
}

public class GetPrescriptionDTO
{
    public int IdPrescription { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<Medicament> Prescriptions { get; set; } = new List<Medicament>();
    public required GetDoctorDTO Doctor { get; set; }
}

public class GetDoctorDTO
{
    public int IdDoctor { get; set; }
    public string FirstName { get; set; } = string.Empty;
}
