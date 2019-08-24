using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace Pck
{
	class ParseAttributeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(InstanceDescriptor) == destinationType)
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(InstanceDescriptor) == destinationType)
			{
				var attr = (ParseAttribute)value;
				return new InstanceDescriptor(typeof(ParseAttribute).GetConstructor(new Type[] { typeof(string), typeof(object) }), new object[] { attr.Name, attr.Value });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(ParseAttributeConverter))]
	public struct ParseAttribute
	{
		public readonly string Name;
		public readonly object Value;
		public ParseAttribute(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}
