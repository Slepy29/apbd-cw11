using System;
using System.Collections.Generic;
using CW10.Models;
using Microsoft.EntityFrameworkCore;

namespace CW10.Data;

public partial class DatabaseContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }

    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.Birthdate)
                .HasColumnType("date");
        });
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.Property(e => e.Date)
                .HasColumnType("date");
        });
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.Property(e => e.DueDate)
                .HasColumnType("date");
        });


        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
            {
                new Doctor {
                    IdDoctor = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "abc@gmail.com"
                },
                new Doctor {
                    IdDoctor = 2,
                    FirstName = "Andrzej",
                    LastName = "Nowak",
                    Email = "def@gmail.com"
                }
            });

        modelBuilder.Entity<Patient>().HasData(new List<Patient>
            {
                new Patient {
                    IdPatient = 1,
                    FirstName = "Grzegorz",
                    LastName = "Nowaczkiewicz",
                    Birthdate = new DateTime(1999,1,1)
                },
                new Patient {
                    IdPatient = 2,
                    FirstName = "Tomasz",
                    LastName = "Kruszyński",
                    Birthdate = new DateTime(1989,2,2)
                }
            });

        modelBuilder.Entity<Prescription>().HasData(new List<Prescription>
            {
                new Prescription {
                    IdPrescription = 1,
                    Date = new DateTime(2001, 3, 4),
                    DueDate = new DateTime(2001, 3, 20),
                    IdPatient = 1,
                    IdDoctor = 1
                },
                new Prescription {
                    IdPrescription = 2,
                    Date = new DateTime(2004, 3, 4),
                    DueDate = new DateTime(2005, 3, 20),
                    IdPatient = 2,
                    IdDoctor = 2
                },
            });

        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>
            {
                new Medicament {
                    IdMedicament = 1,
                    Name = "Ibuprom",
                    Description = "Help's with pain within a hour.",
                    Type = "Painkiller"
                },
                new Medicament {
                    IdMedicament = 2,
                    Name = "Apap",
                    Description = "Help's with pain within a hour.",
                    Type = "Painkiller"
                },
            });

        modelBuilder.Entity<PrescriptionMedication>().HasData(new List<PrescriptionMedication>
            {
                new PrescriptionMedication {
                  IdPrescription = 1,
                  IdMedicament = 1,
                  Details = "Dosage as on receipt of prescripeted drug."
                },
                new PrescriptionMedication {
                  IdPrescription = 2,
                  IdMedicament = 2,
                  Dose = 10,
                  Details = "Don't mix with alcohol"
                },
            });
    }
}
