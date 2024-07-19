using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts
{
    public interface IAuthenticatonService : IAsyncDisposable
    {
        ValueTask<IUser?> CreateUserAsync(string email,string password);
        ValueTask<IUser?> LoginUserAsync(string email, string password);
        ValueTask LogoutUserAsync();
        ValueTask<IUser?> GetLoggedInUserAsync();
        ValueTask<bool> SendPasswordResetLinkAsync(string email);
        ValueTask WatchAuthStateAsync();

        event EventHandler<IUser?>? OnAuthStateChanged;
    }
}
