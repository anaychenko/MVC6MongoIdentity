using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace A365.Identity.MongoDb
{

	// ReSharper disable once UnusedTypeParameter
	public class UserStore<TUser, TRole> :
		IUserLoginStore<TUser>,
		IUserRoleStore<TUser>,
		IUserClaimStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserEmailStore<TUser>,
		IUserLockoutStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IQueryableUserStore<TUser>,
		IUserTwoFactorStore<TUser>
		where TUser : User
		where TRole : Role
	{

		public UserStore(DbContext context, IdentityErrorDescriber describer = null)
		{
			Context = context;
			ErrorDescriber = describer ?? new IdentityErrorDescriber();
		}

		private bool _disposed;

		public DbContext Context { get; }

		public IdentityErrorDescriber ErrorDescriber { get; set; }


		public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.Id.ToString());
		}

		public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return Task.FromResult(user.UserName);
		}

		public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.UserName = userName;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.NormalizedUserName;
			}, cancellationToken);
		}

		public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.NormalizedUserName = normalizedName;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public async virtual Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				Context.SaveUser(user);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public async virtual Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}

				Context.SaveUser(user);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public async virtual Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}

				user.Active = "I";
				Context.SaveUser(user);
				return IdentityResult.Success;
			}, cancellationToken);
		}

		public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				var foundUser = Context.FindUser<TUser>(userId);

				return foundUser;
			}, cancellationToken);
		}

		public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				var foundUser = Context.FindUserByUserName<TUser>(normalizedUserName);

				return foundUser;
			}, cancellationToken);
		}

		public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.PasswordHash = passwordHash;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.PasswordHash;
			}, cancellationToken);
		}

		public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				return user.PasswordHash != null;
			}, cancellationToken);
		}

		public async virtual Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (String.IsNullOrWhiteSpace(roleName))
				{
					throw new ArgumentException("Название роли не можзет быть пустым", "roleName");
				}
				var roleEntity = Context.FindRoleByName<TRole>(roleName);
				if (roleEntity == null)
				{
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Роль {0} не найдена.", roleName));
				}
				if (!user.Roles.Contains(roleEntity.Name))
				{
					user.Roles.Add(roleEntity.Name);
					Context.SaveUser(user);
				}
			}, cancellationToken);
		}

		public async virtual Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (String.IsNullOrWhiteSpace(roleName))
				{
					throw new ArgumentException("Название роли не может быть пустым", "roleName");
				}
				var roleEntity = Context.FindRoleByName<TRole>(roleName);
				if (roleEntity != null)
				{
					if (user.Roles.Contains(roleEntity.Name))
					{
						user.Roles.Remove(roleEntity.Name);
						Context.SaveUser(user);
					}
				}
			}, cancellationToken);
		}

		public virtual async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.Roles;
			}, cancellationToken);
		}

		public virtual async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (string.IsNullOrWhiteSpace(roleName))
				{
					throw new ArgumentException("Название роли не может быть пустым", "roleName");
				}
				var role = Context.FindRoleByName<TRole>(roleName);
				return role != null && user.Roles.Contains(role.Name);
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
		public async virtual Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.Claims;
			}, cancellationToken);
		}

		public async virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (claims == null)
				{
					throw new ArgumentNullException("claims");
				}
				foreach (var claim in claims)
				{
					user.Claims.Add(claim);
				}
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public async virtual Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (claim == null)
				{
					throw new ArgumentNullException("claim");
				}
				if (newClaim == null)
				{
					throw new ArgumentNullException("newClaim");
				}

				if (user.Claims.Contains(claim))
				{
					user.Claims.Remove(claim);
				}
				user.Claims.Add(newClaim);
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public async virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (claims == null)
				{
					throw new ArgumentNullException("claims");
				}
				foreach (var claim in claims)
				{
					user.Claims.Remove(claim);
				}
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				if (login == null)
				{
					throw new ArgumentNullException("login");
				}
				var l = new IdentityUserLogin<ObjectId>
				{
					UserId = user.Id,
					ProviderKey = login.ProviderKey,
					LoginProvider = login.LoginProvider,
					ProviderDisplayName = login.ProviderDisplayName
				};
				user.LoginInfo.Add(l);
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				var userId = user.Id;
				var entry = user.LoginInfo.FirstOrDefault(l => l.UserId.Equals(userId) && l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
				if (entry != null)
				{
					user.LoginInfo.Remove(entry);
					Context.SaveUser(user);
				}
			}, cancellationToken);
		}

		public async virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return await Task.FromResult<IList<UserLoginInfo>>(
				user.LoginInfo.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList()
				);
		}

		public async virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				var ulInfo = new UserLoginInfo(loginProvider, providerKey, null);
				var user = Context.FindUserByLogin<TUser>(ulInfo);
				return user;
			}, cancellationToken);
		}

		public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.EmailConfirmed;
			}, cancellationToken);
		}

		public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.EmailConfirmed = confirmed;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.Email = email;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.Email;
			}, cancellationToken);
		}

		public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.NormalizedEmail;
			}, cancellationToken);
		}

		public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.NormalizedEmail = normalizedEmail;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				var user = Context.FindUserByEmail<TUser>(normalizedEmail);
				return user;
			}, cancellationToken);
		}

		public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.LockoutEnd;
			}, cancellationToken);
		}


		public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.LockoutEnd = lockoutEnd;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.AccessFailedCount++;
				return user.AccessFailedCount;
			}, cancellationToken);
		}

		public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.AccessFailedCount = 0;
			}, cancellationToken);
		}
		public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.AccessFailedCount;
			}, cancellationToken);
		}

		public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.LockoutEnabled;
			}, cancellationToken);
		}

		public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.LockoutEnabled = enabled;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.PhoneNumber = phoneNumber;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.PhoneNumber;
			}, cancellationToken);
		}

		public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.PhoneNumberConfirmed;
			}, cancellationToken);
		}

		public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.PhoneNumberConfirmed = confirmed;
				Context.SaveUser(user);
			}, cancellationToken);
		}


		public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.SecurityStamp = stamp;
				Context.SaveUser(user);
			}, cancellationToken);
		}

		public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.SecurityStamp;
			}, cancellationToken);
		}

		public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				user.TwoFactorEnabled = enabled;
				Context.SaveUser(user);
			}, cancellationToken);
		}


		public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (user == null)
				{
					throw new ArgumentNullException("user");
				}
				return user.TwoFactorEnabled;
			}, cancellationToken);
		}

		public async virtual Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (claim == null)
				{
					throw new ArgumentNullException("claim");
				}
				return Context.GetUsersByClaim<TUser>(claim).ToList();
			}, cancellationToken);
		}


		public async virtual Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				ThrowIfDisposed();
				if (String.IsNullOrEmpty(roleName))
				{
					throw new ArgumentNullException("roleName");
				}

				var role = Context.FindRoleByName<TRole>(roleName);

				if (role != null)
				{
					return Context.GetUsersByRole<TUser>(roleName).ToList();
				}
				return new List<TUser>();
			}, cancellationToken);
		}

		public IQueryable<TUser> Users => Context.Database.GetCollection<TUser>("Users").AsQueryable<TUser>();
	}
}