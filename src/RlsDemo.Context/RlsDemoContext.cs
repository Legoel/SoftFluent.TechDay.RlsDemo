using Microsoft.EntityFrameworkCore;

namespace RslDemo.Context
{
	public class Tenant
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
	}

	public class SensitiveDatum
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Content { get; set; }

		public int TenantId { get; set; }
		public Tenant Tenant { get; set; } = null!;
	}

	public partial class RlsDemoContext : DbContext
	{
		public RlsDemoContext(DbContextOptions<RlsDemoContext> options)
			: base(options)
		{
		}

		public virtual DbSet<Tenant> Tenants { get; set; }

		public virtual DbSet<SensitiveDatum> SensitiveData { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tenant>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();
				entity.HasIndex(e => e.Name).IsUnique();
			});

			modelBuilder.Entity<SensitiveDatum>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();
				entity.HasIndex(e => new { e.Name, e.TenantId }).IsUnique();

				entity.HasOne(e => e.Tenant)
					.WithMany()
					.HasForeignKey(e => e.TenantId)
					.OnDelete(DeleteBehavior.ClientCascade);
			});

			EnsureData(modelBuilder);
		}

		private void EnsureData(ModelBuilder modelBuilder)
		{
			var tenants = new Tenant[] {
				new Tenant{ Name = "Locataire 1", Id = 1},
				new Tenant{ Name = "Locataire 2", Id = 2},
				new Tenant{ Name = "Locataire 3", Id = 3},
			};

			int id = 1;
			var sensitiveData = tenants.SelectMany(tenant => new SensitiveDatum[]
			{
				new SensitiveDatum {Id = id++, Name = "Nom du locataire " + tenant.Id, Content = Faker.Name.FullName(), TenantId = tenant.Id},
				new SensitiveDatum {Id = id++, Name = "Email du locataire " + tenant.Id, Content = Faker.Internet.Email(), TenantId = tenant.Id},
				new SensitiveDatum {Id = id++, Name = "Numéro de sécu du locataire " + tenant.Id, Content = Faker.Identification.SocialSecurityNumber(), TenantId = tenant.Id},
				new SensitiveDatum {Id = id++, Name = "Compte bancaire du locataire " + tenant.Id, Content = Faker.Finance.Isin(), TenantId = tenant.Id},
			});

			modelBuilder.Entity<Tenant>(entity => entity.HasData(tenants));
			modelBuilder.Entity<SensitiveDatum>(entity => entity.HasData(sensitiveData));
		}
	}
}
