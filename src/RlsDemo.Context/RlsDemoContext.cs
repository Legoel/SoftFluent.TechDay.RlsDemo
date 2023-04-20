﻿using Microsoft.EntityFrameworkCore;
using RlsDemo.Context.Model;

namespace RslDemo.Context
{
    public partial class RlsDemoContext : DbContext
	{
        public int ContextTenantId { get; set; }

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
				entity.HasKey(e => e.Identifier);
				entity.Property(e => e.Identifier).ValueGeneratedOnAdd();
				entity.HasIndex(e => e.Name).IsUnique();
			});

			modelBuilder.Entity<SensitiveDatum>(entity =>
			{
				entity.HasKey(e => e.Identifier);
				entity.Property(e => e.Identifier).ValueGeneratedOnAdd();
				entity.HasIndex(e => new { e.Name, e.TenantId }).IsUnique();
				entity.HasQueryFilter(e => e.TenantId == ContextTenantId);

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
				new Tenant{ Name = "Locataire 1", Identifier = 1, TrackCreationTime = DateTime.Now, TrackCreationUser = "System", TrackLastWriteTime = DateTime.Now, TrackLastWriteUser = "System"},
				new Tenant{ Name = "Locataire 2", Identifier = 2, TrackCreationTime = DateTime.Now, TrackCreationUser = "System", TrackLastWriteTime = DateTime.Now, TrackLastWriteUser = "System"},
				new Tenant{ Name = "Locataire 3", Identifier = 3, TrackCreationTime = DateTime.Now, TrackCreationUser = "System", TrackLastWriteTime = DateTime.Now, TrackLastWriteUser = "System"},
			};

			int datumId = 1;
			var sensitiveData = tenants.SelectMany(tenant => EnsureSensitiveDataForTenant(ref datumId, tenant));

			modelBuilder.Entity<Tenant>(entity => entity.HasData(tenants));
			modelBuilder.Entity<SensitiveDatum>(entity => entity.HasData(sensitiveData));
		}

		private IEnumerable<SensitiveDatum> EnsureSensitiveDataForTenant(ref int datumId, Tenant tenant)
		{
			return new List<SensitiveDatum> {
				new SensitiveDatum
				{
					Identifier = datumId++,
					TenantId = tenant.Identifier,
					Type = SensitiveDatumType.Name,
					Name = "Nom du locataire " + tenant.Identifier,
					Content = Faker.Name.FullName(),
					TrackCreationTime = DateTime.Now,
					TrackCreationUser = "System",
					TrackLastWriteTime = DateTime.Now,
					TrackLastWriteUser = "System",
				},
				new SensitiveDatum
				{
					Identifier = datumId++,
					TenantId = tenant.Identifier,
					Type = SensitiveDatumType.Email,
					Name = "Email du locataire " + tenant.Identifier,
					Content = Faker.Internet.Email(),
					TrackCreationTime = DateTime.Now,
					TrackCreationUser = "System",
					TrackLastWriteTime = DateTime.Now,
					TrackLastWriteUser = "System",
				},
				new SensitiveDatum
				{
					Identifier = datumId++,
					TenantId = tenant.Identifier,
					Type = SensitiveDatumType.SocialSecurityNumber,
					Name = "Numéro de sécu du locataire " + tenant.Identifier,
					Content = Faker.Identification.SocialSecurityNumber(),
					TrackCreationTime = DateTime.Now,
					TrackCreationUser = "System",
					TrackLastWriteTime = DateTime.Now,
					TrackLastWriteUser = "System",
				},
				new SensitiveDatum
				{
					Identifier = datumId++,
					TenantId = tenant.Identifier,
					Type = SensitiveDatumType.IsinAccountNumber,
					Name = "Identifiant bancaire du locataire " + tenant.Identifier,
					Content = Faker.Finance.Isin(),
					TrackCreationTime = DateTime.Now,
					TrackCreationUser = "System",
					TrackLastWriteTime = DateTime.Now,
					TrackLastWriteUser = "System",
				}
			};
		}
	}
}
