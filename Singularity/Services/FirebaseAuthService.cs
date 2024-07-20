using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBindGen;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.Communication;
using Singularity.Contracts;
using Singularity.Misc;
using Singularity.Models;

namespace Singularity.Services;

public class FirebaseAuthService : IAuthenticatonService
{
    private Lazy<Task<JObjPtr>> FirebaseJSReference { get; }
    public ILogger<FirebaseAuthService> Logger { get; }

    private static bool _firstInstance = true;
    public FirebaseAuthService(IJSRuntime runtime,ILogger<FirebaseAuthService> logger)
    {
        Logger = logger;
        FirebaseJSReference = new(async() =>
        {
            await BindGen.InitAsync(runtime);
            Logger.LogInformation("creating auth service instance");
            return await BindGen.ImportRefAsync("/js/firebase.js");
        });
    }

    public async ValueTask<IUser?> CreateUserAsync(string email, string password)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.CallAsync<FirebaseUser>("FirebaseCreateUserWithEmailAndPassword", email, password);
            Logger.LogInformation("user created successfully with email"+email);
            return new User(firebaseUser.User.Uid);
        }
        catch (Exception ex)
        {
            var message = FirebaseAuthErrorCodeMapHelper.GetErrorFromException(ex);
            Logger.LogError(ex,"Can't create user");
            throw new AuthException(message);
        }
    }
    public async ValueTask<IUser?> LoginUserAsync(string email, string password)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.CallAsync<FirebaseUser>("FirebaseSignInWithEmailAndPassword", email, password);
            Logger.LogInformation("user logged in successfully with email" + email);
            return new User(firebaseUser.User.Uid);
        }
        catch (Exception ex)
        {
            var message = FirebaseAuthErrorCodeMapHelper.GetErrorFromException(ex);
            Logger.LogError(ex, "Can't login user");
            throw new AuthException(message);
        }
    }


    public async ValueTask<IUser?> GetLoggedInUserAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            var firebaseUser = await module.CallAsync<FirebaseLoggedUser?>("getCurrentUser");

            Logger.LogInformation("user fetched successfully with uid:" + firebaseUser?.Uid);

            if (firebaseUser == null)
                return null;

            return new User(firebaseUser.Uid);
        }
        catch(Exception ex) 
        {
            Logger.LogError(ex, "Can't get current user");
            return null;
        }
    }


    public async ValueTask LogoutUserAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.CallVoidAsync("FirebaseSignOut");
            Logger.LogInformation("user logged out successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Can't logout current user");
        }
    }

    public async ValueTask<bool> SendPasswordResetLinkAsync(string email)
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.CallVoidAsync("FirebaseGeneratePasswordResetLink", email);
            Logger.LogInformation("password reset link sent successfully");
            return true;
        }
        catch(Exception e)
        {
            Logger.LogError(e, "Can't send reset link for "+email);
            return false;
        }
    }

    public async ValueTask WatchAuthStateAsync()
    {
        var module = await FirebaseJSReference.Value;

        try
        {
            await module.CallVoidAsync("subAuthStateChanged", DotNetObjectReference.Create(this));
            Logger.LogInformation("started watching auth state change ");
        }
        catch (Exception e)
        {
            Logger.LogError(e, "cant start watching auth state change ");
        }
    }

    [JSInvokable("authChanged")]
    public void AuthChanged(User? user)
    {
        Logger.LogInformation(" auth state change ->"+user);

        if (_firstInstance)
            OnAuthStateChanged?.Invoke(this, user);
        _firstInstance = false;
    }

    public ValueTask DisposeAsync()
    {
        Logger.LogInformation(" auth desstroyed");
        return ValueTask.CompletedTask;
    }

    public event EventHandler<IUser?>? OnAuthStateChanged;
}
