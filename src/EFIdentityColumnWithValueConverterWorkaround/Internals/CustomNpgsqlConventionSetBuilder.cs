// <copyright file="CustomNpgsqlConventionSetBuilder.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround.Internals
{
	using System;
	using System.Linq;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
	using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
	using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;

	public class CustomNpgsqlConventionSetBuilder : NpgsqlConventionSetBuilder
	{
		private readonly Version postgresVersion;

		public CustomNpgsqlConventionSetBuilder(ProviderConventionSetBuilderDependencies dependencies,
			RelationalConventionSetBuilderDependencies relationalDependencies, INpgsqlOptions npgsqlOptions) : base(dependencies,
			relationalDependencies, npgsqlOptions)
		{
			this.postgresVersion = npgsqlOptions.PostgresVersion;
		}

		public override ConventionSet CreateConventionSet()
		{
			ConventionSet conventionSet = base.CreateConventionSet();

			int index = conventionSet.ModelInitializedConventions.IndexOf(conventionSet.ModelInitializedConventions
				.OfType<NpgsqlValueGenerationStrategyConvention>()
				.Single());

			conventionSet.ModelInitializedConventions[index] =
				new CustomNpgsqlValueGenerationStrategyConvention(Dependencies, RelationalDependencies, this.postgresVersion);

			index = conventionSet.ModelFinalizingConventions.IndexOf(conventionSet.ModelFinalizingConventions
				.OfType<NpgsqlValueGenerationStrategyConvention>()
				.Single());

			conventionSet.ModelFinalizingConventions[index] =
				new CustomNpgsqlValueGenerationStrategyConvention(Dependencies, RelationalDependencies, this.postgresVersion);

			return conventionSet;
		}
	}
}