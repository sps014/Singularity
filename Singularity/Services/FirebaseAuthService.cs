using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.Communication;
using Singularity.Contracts;
using Singularity.Misc;
using Singularity.Models;

namespace Singularity.Services;

public class FirebaseAuthService : IAuthenticatonService
{
    private Lazy<Task<IJSObjectReference>> FirebaseJSReference { get; }
    private static bool _firstInstance = true;
    public FirebaseAuthService(IJSRuntime runtime)
    {
        FirebaseJSReference = new(() => runtime.InvokeAsync<IJSObjectReference>(
                "import", "./js/firebase.js").AsTask());

    }

    public async ValueTask<IUser?> CreateUserAsync(string email, string password)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.InvokeAsync<FirebaseUser>("FirebaseCreateUserWithEmailAndPassword", email, password);
            return new User(firebaseUser.User.Uid);
        }
        catch (Exception ex)
        {
            var message = FirebaseAuthErrorCodeMapHelper.GetErrorFromException(ex);
            throw new AuthException(message);
        }
    }
    public async ValueTask<IUser?> LoginUserAsync(string email, string password)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.InvokeAsync<FirebaseUser>("FirebaseSignInWithEmailAndPassword", email, password);
            return new User(firebaseUser.User.Uid);
        }
        catch (Exception ex)
        {
            var message = FirebaseAuthErrorCodeMapHelper.GetErrorFromException(ex);
            throw new AuthException(message);
        }
    }


    public async ValueTask<IUser?> GetLoggedInUserAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.InvokeAsync<FirebaseLoggedUser?>("getCurrentUser");

            if (firebaseUser == null)
                return null;

            return new User(firebaseUser.Uid);
        }
        catch
        {
            return null;
        }
    }


    public async ValueTask LogoutUserAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.InvokeVoidAsync("FirebaseSignOut");
        }
        catch
        {
        }
    }

    public async ValueTask<bool> SendPasswordResetLinkAsync(string email)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.InvokeVoidAsync("FirebaseGeneratePasswordResetLink", email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask WatchAuthStateAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.InvokeVoidAsync("subAuthStateChanged", DotNetObjectReference.Create(this));
        }
        catch
        {
        }
    }

    [JSInvokable("authChanged")]
    public void AuthChanged(User? user)
    {
        if (_firstInstance)
            OnAuthStateChanged?.Invoke(this, user);
        _firstInstance = false;
    }

    public async ValueTask DisposeAsync()
    {
        if (FirebaseJSReference.IsValueCreated)
            await (await FirebaseJSReference.Value).DisposeAsync();
    }

    public event EventHandler<IUser?>? OnAuthStateChanged;
}
