using System;
namespace Plugins.Beebyte.Obfuscator
{
	/**
	 * Suppresses certain messages (usually warnings) that the Obfuscator can output.
	 */
	[AttributeUsage(AttributeTargets.Method)]
	public class SuppressLogAttribute : System.Attribute
	{
#pragma warning disable 414
		private readonly MessageCode messageCode;
#pragma warning restore 414

		private SuppressLogAttribute()
		{
		}

		public SuppressLogAttribute(MessageCode messageCode)
		{
			this.messageCode = messageCode;
		}
	}
}
