using System;

namespace A365.Identity.MongoDb
{
	public class ConfigurationException : Exception
	{
		public ConfigurationException(string message) : base(message) {}
	}
}