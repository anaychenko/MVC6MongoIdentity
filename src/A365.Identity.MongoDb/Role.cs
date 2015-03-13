using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace A365.Identity.MongoDb
{
	public class Role : IdentityRole<ObjectId>
	{
		public char Active
		{
			get;
			set;
		}
	}
}