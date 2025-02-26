using Assignment2.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment2
{
    public class Context : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("Asset");
            });

            modelBuilder.Entity<Asset_Type>(entity =>
            {
                entity.ToTable("Asset_Type");
            });

            modelBuilder.Entity<Certification>(entity =>
            {
                entity.ToTable("Certification");
            });

            modelBuilder.Entity<Contact_Information>(entity =>
            {
                entity.ToTable("Contact_Information");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");
            });

            modelBuilder.Entity<Customer_Invoice>(entity =>
            {
                entity.ToTable("Customer_Invoice");
            });

            modelBuilder.Entity<Customer_Service>(entity =>
            {
                entity.ToTable("Customer_Service");
            });

            modelBuilder.Entity<Customer_Service_Scheduled>(entity =>
            {
                entity.ToTable("Customer_Service_Scheduled");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
            });

            modelBuilder.Entity<Employee_Certification>(entity =>
            {
                entity.ToTable("Employee_Certification");
                entity.HasKey(ec => new { ec.EmployeeId, ec.CertificationId });

            });

            modelBuilder.Entity<Employee_Service_Assignment>(entity =>
            {
                entity.ToTable("Employee_Service_Assignment");
                entity.HasKey(e => new { e.Customer_Service_ScheduledId, e.EmployeeId });
            });

            modelBuilder.Entity<Employee_Sick_Leave>(static entity =>
            {
                entity.ToTable("Employee_Sick_Leave");
                entity.HasNoKey();
            });

            modelBuilder.Entity<Employee_Vacation>(entity =>
            {
                entity.ToTable("Employee_Vacation");
            });

            modelBuilder.Entity<Facility>(entity =>
            {
                entity.ToTable("Facility");
            });

            modelBuilder.Entity<Org>(entity =>
            {
                entity.ToTable("Org");
            });

            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.ToTable("Payroll");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");
            });

            modelBuilder.Entity<Rental_Invoice>(entity =>
            {
                entity.ToTable("Rental_Invoice");     
                
            });

            modelBuilder.Entity<Renter_Asset_Agreement>(entity =>
            {
                entity.ToTable("Renter_Asset_Agreement");
            });

            modelBuilder.Entity<Shift_Schedule>(entity =>
            {
                entity.ToTable("Shift_Schedule");
                entity.HasNoKey();
            });

            modelBuilder.Entity<StAddress>(entity =>
            {
                entity.ToTable("StAddress");
            });

            modelBuilder.Entity<Stat_Holidays>(entity =>
            {
                entity.ToTable("Stat_Holidays");
                entity.HasKey(e => e.DateTime);
            });


            // Models
        }

        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<Asset_Type> Asset_Types { get; set; } = null!;
        public DbSet<Certification> Certifications { get; set; } = null!;
        public DbSet<Contact_Information> Contact_Informations { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Customer_Invoice> Customer_Invoices { get; set; } = null!;
        public DbSet<Customer_Service> Customer_Services { get; set; } = null!;
        public DbSet<Customer_Service_Scheduled> Customer_Service_Scheduleds { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<EmployeeDetails> EmployeesDetails { get; set; } // DTO for raw SQL queries
        public DbSet<Employee_Certification> Employee_Certifications { get; set; } = null!;
        public DbSet<Employee_Service_Assignment> Employee_Service_Assignments { get; set; } = null!;
        public DbSet<Employee_Sick_Leave> Employee_Sick_Leaves { get; set; } = null!;
        public DbSet<Employee_Vacation> Employee_Vacations { get; set; } = null!;
        public DbSet<Facility> Facilities { get; set; } = null!;
        public DbSet<Org> Orgs { get; set; } = null!;
        public DbSet<Payroll> Payrolls { get; set; } = null!;
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Rental_Invoice> Rental_Invoices { get; set; } = null!;
        public DbSet<Renter_Asset_Agreement> Renter_Asset_Agreements { get; set; } = null!;
        public DbSet<Shift_Schedule> Shift_Schedules { get; set; } = null!;
        public DbSet<ShiftScheduleDetails> ShiftScheduleDetails { get; set; } = null!; // DTO for raw SQL queries
        public DbSet<StAddress> StAddresses { get; set; } = null!;
        public DbSet<Stat_Holidays> Stat_Holidays { get; set; } = null!;



        public IConfiguration _config { get; set; }

        public Context(IConfiguration configuration) {
            _config = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
        }
    }
}
