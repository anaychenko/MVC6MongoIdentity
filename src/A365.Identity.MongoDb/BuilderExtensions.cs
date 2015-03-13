using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;

namespace A365.Identity.MongoDb
{
	public class MongoIdentityOptions
	{
		public string ConnectionString { get; set; }
		public string IdentityDatabase { get; set; }
		public string UserTableName { get; set; }
		public string RoleTableName { get; set; }
	}

	public static class BuilderExtensions
	{
		public static IdentityBuilder AddMongoIdentity<TUser, TRole>(this IServiceCollection services,IConfiguration config,
			Action<IdentityOptions> configureOptions = null,
			bool useDefaultSubKey = true) 
			where TUser : User
			where TRole : Role
		{
			var builder = services.AddIdentity<TUser, TRole>(config, configureOptions, useDefaultSubKey);
            builder.Services.AddSingleton<DbContext>();
			builder.Services.Add(GetDefaultServices(builder.UserType, builder.RoleType));
			builder.Services.Configure<MongoIdentityOptions>(config.GetSubKey("MongoIdentity"));
            return builder;
		}

		public static IEnumerable<IServiceDescriptor> GetDefaultServices(Type userType, Type roleType)
		{
			var describe = new ServiceDescriber();
			var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
			var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

			yield return describe.Scoped(
				typeof(IUserStore<>).MakeGenericType(userType),
				userStoreType);
			yield return describe.Scoped(
				typeof(IRoleStore<>).MakeGenericType(roleType),
				roleStoreType);
		}
	}

}
