using CW10.DTOs;
using CW10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW10.Endpoints;

public static class EndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/api/prescription", async (CreatePrescriptionDTO prescriptionDTO, [FromServices] IDbService db) =>
        {
            if (prescriptionDTO.DueDate < prescriptionDTO.Date)
            {
                return Results.BadRequest("Due date has to be larger or equal to date");
            }

            if (prescriptionDTO.Medicaments.Count() > 10)
            {
                return Results.BadRequest("Receipt can only containt 10 medicaments");
            }

            foreach (var med in prescriptionDTO.Medicaments)
            {
                if (await db.GetMedicament(med.IdMedicament) == null)
                {
                    return Results.BadRequest("Med " + med.IdMedicament + " doesn't exist");
                }
            }

            if (!await db.DoctorExists(prescriptionDTO.Doctor.IdDoctor))
            {
                return Results.BadRequest("Doctor like this doesn't exist");
            }

            if (await db.PatientExists(prescriptionDTO.Patient.IdPatient))
            {
                await db.CreatePatient(prescriptionDTO.Patient);
            }

            var pre = await db.CreatePrescription(prescriptionDTO.DueDate, prescriptionDTO.Date, prescriptionDTO.Patient, prescriptionDTO.Doctor);

            foreach (var med in prescriptionDTO.Medicaments)
            {
                await db.AssignMedicament(pre, await db.GetMedicament(med.IdMedicament), med.Dose, med.Description);
            }

            return Results.Created();
        });

        app.MapGet("/api/patient/{id}", async (int id, [FromServices] IDbService db) =>
    {
        var patient = await db.GetPatient(id);
        if (patient == null)
        {
            return Results.NotFound();
        }

        var prescriptions = await db.GetPrescriptions(id);

        var patientDTO = new GetPatientDTO
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.Birthdate,
            Prescriptions = prescriptions.Select(p => new GetPrescriptionDTO
            {
                IdPrescription = p.IdPrescription,
                DueDate = p.DueDate,
                Date = p.Date,
                Prescriptions = p.PrescriptionMedications.Select(pm => pm.Medicament),
                Doctor = new GetDoctorDTO
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName
                }
            }).ToList()
        };
        return Results.Ok(patientDTO);
    });

        app.MapPost("/api/register", async (RegisterUserDTO registerUserDTO, [FromServices] IDbService db) =>
        {
            await db.RegisterUser(registerUserDTO);
            return Results.Ok();
        });

        app.MapPost("/api/login", async (LoginUserDTO loginUserDTO, [FromServices] IDbService db) =>
        {
            try
            {
                var tokens = await db.Login(loginUserDTO);
                return Results.Ok(tokens);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });

        app.MapPost("/api/refresh", async (string refreshToken, [FromServices] IDbService db) =>
        {
            try
            {
                var tokens = await db.RefreshToken(refreshToken);
                return Results.Ok(tokens);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });
    }

}

