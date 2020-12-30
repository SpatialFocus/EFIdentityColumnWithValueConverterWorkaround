// <copyright file="MyContext.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround
{
	using EFIdentityColumnWithValueConverterWorkaround.Internals;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public class MyContext : DbContext
	{
		public MyContext(DbContextOptions<MyContext> options) : base(options)
		{
		}

		public DbSet<MyEntity> MyEntities { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ValueConverter<MyEntityId, int> converter = new ValueConverter<MyEntityId, int>(v => v.Value, v => new MyEntityId(v),
				new ConverterMappingHints(valueGeneratorFactory: (p, t) => new TemporaryTypedIdValueGenerator<MyEntityId>()));

			modelBuilder
				.Entity<MyEntity>()
				.Property(p => p.Id)
				.HasConversion(converter)
				.UseIdentityColumnWorkaround();
		}
	}
}