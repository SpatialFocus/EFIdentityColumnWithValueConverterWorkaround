// <copyright file="CustomNpgsqlValueGenerationStrategyConvention.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround.Internals
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions;
	using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
	using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

	public class CustomNpgsqlValueGenerationStrategyConvention : IModelInitializedConvention, IModelFinalizingConvention
	{
		private readonly Version postgresVersion;

		public CustomNpgsqlValueGenerationStrategyConvention(ProviderConventionSetBuilderDependencies dependencies,
			RelationalConventionSetBuilderDependencies relationalDependencies, Version postgresVersion)
		{
			Dependencies = dependencies;
			this.postgresVersion = postgresVersion;
		}

		protected virtual ProviderConventionSetBuilderDependencies Dependencies { get; }

		public virtual void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
			IConventionContext<IConventionModelBuilder> context)
		{
			foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
			{
				foreach (IConventionProperty property in entityType.GetDeclaredProperties())
				{
					NpgsqlValueGenerationStrategy? strategy = null;
					string table = entityType.GetTableName();

					if (table != null)
					{
						StoreObjectIdentifier storeObject = StoreObjectIdentifier.Table(table, entityType.GetSchema());
						strategy = property.GetValueGenerationStrategy(storeObject);

						if (strategy == NpgsqlValueGenerationStrategy.None && !IsStrategyNoneNeeded(property, storeObject))
						{
							strategy = null;
						}
					}
					else
					{
						string view = entityType.GetViewName();

						if (view != null)
						{
							StoreObjectIdentifier storeObject = StoreObjectIdentifier.View(view, entityType.GetViewSchema());
							strategy = property.GetValueGenerationStrategy(storeObject);

							if (strategy == NpgsqlValueGenerationStrategy.None && !IsStrategyNoneNeeded(property, storeObject))
							{
								strategy = null;
							}
						}
					}

					// Needed for the annotation to show up in the model snapshot
					NpgsqlPropertyBuilderExtensions.HasValueGenerationStrategy(property.Builder, strategy);
				}
			}

			static bool IsStrategyNoneNeeded(IProperty property, StoreObjectIdentifier storeObject)
			{
				if (property.ValueGenerated == ValueGenerated.OnAdd && property.GetDefaultValue(storeObject) == null &&
					property.GetDefaultValueSql(storeObject) == null && property.GetComputedColumnSql(storeObject) == null &&
					property.DeclaringEntityType.Model.GetValueGenerationStrategy() != NpgsqlValueGenerationStrategy.None)
				{
					Type providerClrType = (property.GetValueConverter() ?? property.FindRelationalTypeMapping(storeObject)?.Converter)
						?.ProviderClrType.UnwrapNullableType();

					return providerClrType != null && providerClrType.IsInteger();
				}

				return false;
			}
		}

		public virtual void ProcessModelInitialized(IConventionModelBuilder modelBuilder,
			IConventionContext<IConventionModelBuilder> context)
		{
			modelBuilder.HasValueGenerationStrategy(this.postgresVersion < new Version(10, 0)
				? NpgsqlValueGenerationStrategy.SerialColumn
				: NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
		}
	}
}