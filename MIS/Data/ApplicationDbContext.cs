using Microsoft.EntityFrameworkCore;
using MIS.Data.DTO;
using MIS.Data.Models;

namespace MIS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<SpecialityGetModel> Specialiti { get; set; }
        public DbSet<PatientModel> patientModels { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<PatientDiagnosis> PatientDiagnoses { get; set; }
        public DbSet<InspectionPatient> InspectionPatient { get; set; }
        public DbSet<InspectionDoctor> InspectionDoctor{ get; set; }
        public DbSet<DiagnosisModel> DiagnosisModel { get; set; }
        public DbSet<ConsultationModel> ConsultationModels { get; set; }
        public DbSet<InspectionCommentModel> InspectionCommentModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.FullName).IsUnique();
            modelBuilder.Entity<Inspection>().HasKey(x => x.Id);
        }
    }
}


