using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CW10.Data;
using CW10.DTOs;
using CW10.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CW10.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    public DbService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task CreatePatient(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
    }

    public async Task<Prescription> CreatePrescription(DateTime dueDate, DateTime date, Patient patient, Doctor doctor)
    {
        var pre = new Prescription
        {
            Date = date,
            DueDate = dueDate,
            IdPatient = patient.IdPatient,
            IdDoctor = doctor.IdDoctor,
        };

        await _context.Prescriptions.AddAsync(pre);

        return pre;
    }

    public async Task<Medicament> GetMedicament(int id)
    {
        return await _context.Medicaments.FirstAsync(m => m.IdMedicament == id);
    }

    public async Task<bool> DoctorExists(int id)
    {
        var doc = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == id);

        return doc == null;
    }

    public async Task<bool> PatientExists(int id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(m => m.IdPatient == id);

        return patient == null;
    }

    public async Task AssignMedicament(Prescription prescription, Medicament medicament, int? dose, string details)
    {
        await _context.PrescriptionMedications.AddAsync(new PrescriptionMedication
        {
            IdMedicament = medicament.IdMedicament,
            IdPrescription = prescription.IdPrescription,
            Dose = dose,
            Details = details,
        });

    }

    public async Task<IEnumerable<Prescription>> GetPrescriptions(int id)
    {
        return await _context.Prescriptions.Include(p => p.Doctor).Include(p => p.PrescriptionMedications).ThenInclude(p => p.Medicament).Where(p => p.IdPatient == id).Include(p => p.Patient).ToListAsync();
    }

    public async Task<Patient?> GetPatient(int id)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.IdPatient == id);
    }


    public async Task RegisterUser(RegisterUserDTO user)
    {
        _context.Users.Add(new User
        {
            Username = user.Username,
            PasswordHash = HashPassword(user.Password),
        });
        await _context.SaveChangesAsync();
    }

    public async Task<TokenResponseDTO> Login(LoginUserDTO loginUserDTO)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUserDTO.Username);
        if (user == null || !VerifyPassword(user.PasswordHash, loginUserDTO.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(user);

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponseDTO> RefreshToken(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(user);

        return new TokenResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8);

        byte[] hashBytes = new byte[salt.Length + subkey.Length];
        Array.Copy(salt, 0, hashBytes, 0, salt.Length);
        Array.Copy(subkey, 0, hashBytes, salt.Length, subkey.Length);

        return Convert.ToBase64String(hashBytes);
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Username)
        }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public bool VerifyPassword(string storedHash, string password)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, salt.Length);

        byte[] storedSubkey = new byte[hashBytes.Length - salt.Length];
        Array.Copy(hashBytes, salt.Length, storedSubkey, 0, storedSubkey.Length);

        byte[] incomingSubkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8);

        return storedSubkey.SequenceEqual(incomingSubkey);
    }
}
