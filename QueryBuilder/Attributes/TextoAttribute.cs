using System;

namespace QueryBuilder.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class TextoAttribute : Attribute
	{
		public string Texto { get; set; }

		public TextoAttribute(string texto)
		{
			this.Texto = texto;
		}
	}
}
