using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,AllowMultiple =true,Inherited =false)]
	public sealed class TransformAttribute : Attribute
	{
		public TransformAttribute(string name,string fromExtension, string toExtension,string description) :this(name)
		{
			FromExtension = fromExtension;
			ToExtension = toExtension;
			Description = description;
		}
		public TransformAttribute(string name)
		{
			if (null == name)
				throw new ArgumentNullException("name");
			if ("" == name)
				throw new ArgumentException("The name cannot be empty.","name");
			Name = name;
		}

		public string Name { get; set; } = null;
		public string FromExtension { get; set; } = null;
		public string ToExtension { get; set; } = null;
		public string Description { get; set; } = null;
 	}
}
