// <copyright file="TemporaryTypedIdValueGenerator.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround
{
	using System;
	using System.Threading;
	using Microsoft.EntityFrameworkCore.ChangeTracking;
	using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

	public class TemporaryTypedIdValueGenerator<TTypedId> : TemporaryNumberValueGenerator<TTypedId>
	{
		private int current = int.MinValue + 1000;

		public override TTypedId Next(EntityEntry entry)
		{
			return (TTypedId)Activator.CreateInstance(typeof(TTypedId), Interlocked.Increment(ref this.current));
		}
	}
}