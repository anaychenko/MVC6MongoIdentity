using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace A365.Identity.MongoDb
{
	public class User : IdentityUser<ObjectId>
	{

	public string Active { get; set; }
        public List<IdentityUserLogin<ObjectId>> LoginInfo { get; set; }
		public List<Claim> Claims { get; set; }
		public List<string> Roles { get; set; }


		public User()
		{
			Claims = new List<Claim>();
			Roles = new List<string>();
			LoginInfo = new List<IdentityUserLogin<ObjectId>>();
		}
	}
}