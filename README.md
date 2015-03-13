# MVC-6 MongoDb Identity support
Using:

1. Add MongoDb connеction info in config.json
```json
{
	"MongoIdentity": {
		"ConnectionString": "mongodb://localhost",
		"IdentityDatabase": "MongoIdentity",
		"UserTableName": "Users",   **Optional
		"RoleTableName":  "Roles"   **Optional
	}
}
```
2. Create models: AccountViewModels.cs
```c#
public class ApplicationUser : User
{
	public string FirstName { get; set; }
	public string LastName { get; set; } 
	public DataType BirthDay { get; set; }
}

public class ApplicationRole : Role
{
	public string Desription { get; set; }
}
```
2. Add Identity services to the services container Startup.cs
```c#
public void ConfigureServices(IServiceCollection services)
{
	// Add Identity services to the services container.
	services.AddMongoIdentity<ApplicationUser, ApplicationRole>(Configuration);

    // Add MVC services to the services container.
    services.AddMvc();

    // Uncomment the following line to add Web API servcies which makes it easier to port Web API 2 controllers.
    // You need to add Microsoft.AspNet.Mvc.WebApiCompatShim package to project.json
    // services.AddWebApiConventions();
}
```



