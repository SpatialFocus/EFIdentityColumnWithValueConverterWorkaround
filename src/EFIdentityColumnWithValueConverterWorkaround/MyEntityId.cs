// <copyright file="MyEntityId.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace EFIdentityColumnWithValueConverterWorkaround
{
	public class MyEntityId : TypedIdValueBase
	{
		public MyEntityId(int value) : base(value)
		{
		}
	}
}