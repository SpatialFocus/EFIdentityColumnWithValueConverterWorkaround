// <copyright file="SharedTypeExtensions.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround.Internals
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	internal static class SharedTypeExtensions
	{
		public static PropertyInfo GetAnyProperty(this Type type, string name)
		{
			List<PropertyInfo> props = type.GetRuntimeProperties().Where(p => p.Name == name).ToList();

			if (props.Count() > 1)
			{
				throw new AmbiguousMatchException();
			}

			return props.SingleOrDefault();
		}

		public static bool IsInteger(this Type type)
		{
			type = type.UnwrapNullableType();

			return type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(uint) ||
				type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte);
		}

		public static bool IsNullableType(this Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();

			return !typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static bool IsPrimitive(this Type type) => type.IsInteger() || type.IsNonIntegerPrimitive();

		public static Type MakeNullable(this Type type) => type.IsNullableType() ? type : typeof(Nullable<>).MakeGenericType(type);

		public static Type UnwrapEnumType(this Type type) => type.GetTypeInfo().IsEnum ? Enum.GetUnderlyingType(type) : type;

		public static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

		private static bool IsNonIntegerPrimitive(this Type type)
		{
			type = type.UnwrapNullableType();

			return type == typeof(bool) || type == typeof(byte[]) || type == typeof(char) || type == typeof(DateTime) ||
				type == typeof(DateTimeOffset) || type == typeof(decimal) || type == typeof(double) || type == typeof(float) ||
				type == typeof(Guid) || type == typeof(string) || type == typeof(TimeSpan) || type.GetTypeInfo().IsEnum;
		}
	}
}