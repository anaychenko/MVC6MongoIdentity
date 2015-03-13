using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.ConfigurationModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace A365.Identity.MongoDb
{
	public class DbContext : IDisposable
	{
		private readonly string _userTableName = "Users";
		private readonly string _roleTableName = "Roles";
		public MongoDatabase Database { get; set; }

		private DbContext(string serverName, string databaseName, string userTableName, string roleTableName)
		{
			if (!string.IsNullOrWhiteSpace(userTableName))
				_userTableName = userTableName;
			if (!string.IsNullOrWhiteSpace(roleTableName))
				_roleTableName = roleTableName;
			var client = new MongoClient(serverName);
			var server = client.GetServer();
			Database = server.GetDatabase(databaseName);
		}

		public void SaveUser(User user)
		{
			Database.GetCollection(_userTableName).Save(user);
		}

		public TUser FindUser<TUser>(string id) where TUser : User
		{
			var search = Query.EQ("_id", ObjectId.Parse(id));
			var foundUser = Database.GetCollection<TUser>(_userTableName).FindOne(search);
			return foundUser;
		}

		public TUser FindUserByUserName<TUser>(string userName) where TUser : User
		{
			var c = Database.GetCollection<TUser>(_userTableName).AsQueryable<TUser>();
			var foundUser = c.FirstOrDefault(i => i.NormalizedUserName.ToUpper() == userName);
			return foundUser;

		}

		public void SaveRole<TRole>(TRole role) where TRole : Role
        {
			Database.GetCollection(_roleTableName).Save(role);
		}

		public TRole FindRole<TRole>(string id) where TRole : Role
		{
			var search = Query.EQ("Id", id);
			var foundRole = Database.GetCollection<TRole>(_roleTableName).FindOne(search);
			return foundRole;
		}

		public TRole FindRoleByName<TRole>(string name) where TRole : Role
        {
			var search = Query.EQ("Name", name);
			var foundRole = Database.GetCollection<TRole>(_roleTableName).FindOne(search);
			return foundRole;
		}

		public TUser FindUserByEmail<TUser>(string email) where TUser : User
        {
			var search = Query.EQ("NormalizedEmail", email);
			var foundRole = Database.GetCollection<TUser>(_userTableName).FindOne(search);
			return foundRole;
		}

		public TUser FindUserByLogin<TUser>(UserLoginInfo userInfo) where TUser : User
        {
			var search = Query.ElemMatch("LoginInfo", Query.And(Query.EQ("LoginProvider", userInfo.LoginProvider), Query.EQ("ProviderKey", userInfo.ProviderKey)));
			var foundUser = Database.GetCollection<TUser>(_userTableName).FindOne(search);
			return foundUser;
		}
		public void Dispose()
		{
			Database = null;
		}

		public static DbContext CreateContext(IConfiguration config)
		{
			const string serverConfig = "MongoIdentity:ConnectionString";
			const string databaseConfig = "MongoIdentity:IdentityDatabase";
			const string userTableNameConfig = "MongoIdentity:UserTableName";
			const string roleTableNameConfig = "MongoIdentity:RoleTableName";

			const string exceptionMessage = "Параметр config.json '{0}' не определен.";

			var server = config.Get(serverConfig);
			if (string.IsNullOrWhiteSpace(server))
				throw new ConfigurationException(string.Format(exceptionMessage, serverConfig));

			var database = config.Get(databaseConfig);
			if (string.IsNullOrWhiteSpace(database))
				throw new ConfigurationException(string.Format(exceptionMessage, databaseConfig));

			return new DbContext(server, database, config.Get(userTableNameConfig), config.Get(roleTableNameConfig));
		}

		public IList<TUser> GetUsersByClaim<TUser>(Claim claim) where TUser : User
        {
			var c = Database.GetCollection<TUser>(_userTableName).AsQueryable();
			return c.Where(i => i.Claims.Any(j => j.Value == claim.Value && j.ValueType == claim.ValueType)).ToList();
		}

		public IList<TUser> GetUsersByRole<TUser>(string roleName) where TUser : User
        {
			var c = Database.GetCollection<TUser>(_userTableName).AsQueryable();
			return c.Where(i => i.Roles.Contains(roleName)).ToList();
		}
	}
}