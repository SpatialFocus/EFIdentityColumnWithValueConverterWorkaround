// <copyright file="TypedIdValueBase.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround
{
	using System;

	public abstract class TypedIdValueBase : IEquatable<TypedIdValueBase>
	{
		protected TypedIdValueBase(int value)
		{
			Value = value;
		}

		public int Value { get; }

		public static bool operator ==(TypedIdValueBase? obj1, TypedIdValueBase? obj2)
		{
			if (object.Equals(obj1, null))
			{
				if (object.Equals(obj2, null))
				{
					return true;
				}

				return false;
			}

			return obj1.Equals(obj2);
		}

		public static bool operator !=(TypedIdValueBase x, TypedIdValueBase y)
		{
			return !(x == y);
		}

		public bool Equals(TypedIdValueBase? other)
		{
			return Value == other?.Value;
		}

		public override bool Equals(object? obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}

			return obj is TypedIdValueBase other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return $"{GetType().Name}: {Value}";
		}
	}
}