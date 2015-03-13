using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace A365.Identity.MongoDb
{
	public class RoleStore<TRole, TContext, TKey> :
		IQueryableRoleStore<TRole>,
		IRoleClaimStore<TRole>
		where TRole : Role
		where TKey : IEquatable<TKey>
		where TContext : DbContext
	{
		public RoleStore(TContext context, IdentityErrorDescriber describer = null)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			Context = context;
			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}

		private bool _disposed;


		public TContext Context { get; }

		public IdentityErrorDescriber ErrorDescriber { get; set; }


		public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				Context.SaveRole(role);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				Context.SaveRole(role);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				///TODO: Добавить поддержку логического удаления
				role.Active = 'I';
				Context.SaveRole(role);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				return role.Id.ToString();
			}, cancellationToken);
		}

		public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				return role.Name;
			}, cancellationToken);
		}

		public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				role.Name = roleName;
				Context.SaveRole(role);
			}, cancellationToken);
		}


		public virtual Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				return Context.FindRole<TRole>(id);
			}, cancellationToken);
		}

		public virtual Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				return Context.FindRoleByName<TRole>(normalizedName);
			}, cancellationToken);
		}

		public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException(nameof(role));
				}
				return role.NormalizedName;
			}, cancellationToken);
		}

		public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException(nameof(role));
				}
				role.NormalizedName = normalizedName;
				Context.SaveRole(role);
			}, cancellationToken);
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		public void Dispose()
		{
			_disposed = true;
		}

		public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				return role.Claims.Select(i=>new Claim(i.ClaimType,i.ClaimValue)).ToList();
			}, cancellationToken);
		}

		public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			return  Task.Run(() =>
			{
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				if (claim == null)
				{
					throw new ArgumentNullException("claim");
				}
				role.Claims.Add(new IdentityRoleClaim<ObjectId>
				{
					RoleId = role.Id,
					ClaimType = claim.ValueType,
					ClaimValue = claim.Value
				});
				Context.SaveRole(role);
			}, cancellationToken);

		}

		public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			 await Task.Run(() =>
			{
				ThrowIfDisposed();
				if (role == null)
				{
					throw new ArgumentNullException("role");
				}
				if (claim == null)
				{
					throw new ArgumentNullException("claim");
				}
				var c = role.Claims.FirstOrDefault(i => i.ClaimType == claim.ValueType && i.ClaimValue == claim.Value);
				if (c != null)
				{
					role.Claims.Remove(c);
					Context.SaveRole(role);
				}
			}, cancellationToken);

		}

		public IQueryable<TRole> Roles => Context.Database.GetCollection<TRole>("Roles").AsQueryable<TRole>();

		
	}
	public class RoleStore : RoleStore<Role>
	{
		public RoleStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }
	}
	public class RoleStore<TRole> : RoleStore<TRole, DbContext, string>
	   where TRole : Role
	{
		public RoleStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }
	}

	public class RoleStore<TRole, TContext> : RoleStore<TRole, TContext, string>
		where TRole : Role
		where TContext : DbContext
	{
		public RoleStore(TContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }
	}
}