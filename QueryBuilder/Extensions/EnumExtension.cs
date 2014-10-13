
using System;
using System.Linq;
using System.Reflection;
using QueryBuilder.Attributes;

namespace QueryBuilder.Extensions
{
	public static class EnumExtension
	{
		public static string Texto(this Enum field)
		{
			FieldInfo info = field.GetType().GetField(field.ToString());
			TextoAttribute attr = info.GetCustomAttributes(typeof(TextoAttribute), true).FirstOrDefault() as TextoAttribute;

			return attr != null ? attr.Texto : string.Empty;
		}
	}
}
