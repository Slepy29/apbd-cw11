using CW10.Models;
using CW10.DTOs;

namespace CW10.Services;

public interface IDbService
{
    Task<Medicament> GetMedicament(int id);
    Task<Patient?> GetPatient(int id);
    Task<bool> PatientExists(int id);
    Task<bool> DoctorExists(int id);
    Task CreatePatient(Patient patient);
    Task<Prescription> CreatePrescription(DateTime dueDate, DateTime date, Patient patient, Doctor doctor);
    Task AssignMedicament(Prescription prescription, Medicament medicament, int? dose, string details);
    Task<IEnumerable<Prescription>> GetPrescriptions(int id);
    Task RegisterUser(RegisterUserDTO user);
    Task<TokenResponseDTO> Login(LoginUserDTO loginUserDTO);
    Task<TokenResponseDTO> RefreshToken(string refreshToken);
}
