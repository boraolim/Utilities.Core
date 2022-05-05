using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TestUtilities.DBSample.ContextDb
{
    // dotnet ef migrations add Initial --startup-project Castle.DynamicLinqQueryBuilder.DBSample.csproj -c "AppDbContext"
    // dotnet ef database update -c "AppDbContext"
    // dotnet ef dbcontext scaffold "Server=localhost;Database=Prueba;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -o Temp -c "DbAppContext" -f
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) =>
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                              .AddJsonFile("AppSettings.json").Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("country");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.List)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("list");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.PostalZip)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("postalZip");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasIndex(e => e.StoreId, "IX_Product_StoreId");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fK_Detail");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.StoreId).ValueGeneratedNever();

                entity.Property(e => e.OpenDate).HasColumnType("datetime");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18, 0)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
