using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;

namespace Pck
{
	class CharDfaEntryConverter : TypeConverter
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
				var dte = (CharDfaEntry)value;
				return new InstanceDescriptor(typeof(CharDfaEntry).GetConstructor(new Type[] { typeof(int), typeof(CharDfaTransitionEntry[]) }), new object[] { dte.AcceptSymbolId, dte.Transitions});
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(CharDfaEntryConverter))]
	public struct CharDfaEntry
	{
		public CharDfaEntry(int acceptSymbolId, CharDfaTransitionEntry[] transitions)
		{
			AcceptSymbolId = acceptSymbolId;
			Transitions = transitions;
		}
		public int AcceptSymbolId;
		public CharDfaTransitionEntry[] Transitions;
	}
	class CharDfaTransitionEntryConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(InstanceDescriptor) == destinationType)
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(typeof(InstanceDescriptor)==destinationType)
			{
				var dte = (CharDfaTransitionEntry)value;
				return new InstanceDescriptor(typeof(CharDfaTransitionEntry).GetConstructor(new Type[] { typeof(char[]),typeof(int)}),new object[] { dte.PackedRanges, dte.Destination });
			} 
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(CharDfaTransitionEntryConverter))]
	public struct CharDfaTransitionEntry
	{
		public CharDfaTransitionEntry(char[] transitions,int destination)
		{
			PackedRanges = transitions;
			Destination = destination;
		}
		public char[] PackedRanges;
		public int Destination;
	}
}
