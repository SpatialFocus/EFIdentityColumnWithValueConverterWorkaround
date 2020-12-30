// <copyright file="Program.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround
{
	using System;
	using EFIdentityColumnWithValueConverterWorkaround.Internals;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
	using Microsoft.Extensions.DependencyInjection;

	public static class Program
	{
		public static void Main()
		{
			Console.WriteLine("Hello World!");

			ServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddDbContext<MyContext>(options =>
			{
				options.LogTo(Console.WriteLine);

				options.ReplaceService<IProviderConventionSetBuilder, CustomNpgsqlConventionSetBuilder>();

				options.UseNpgsql("Server=localhost;Port=5432;Database=workaround;UserID=postgres;Password=postgres;");
			});

			ServiceProvider buildServiceProvider = serviceCollection.BuildServiceProvider();

			MyContext context = buildServiceProvider.GetRequiredService<MyContext>();
			context.Database.EnsureCreated();

			MyEntity myEntity = new MyEntity();
			context.MyEntities.Add(myEntity);
			context.SaveChanges();

			Console.WriteLine(myEntity.Id);
		}
	}
}