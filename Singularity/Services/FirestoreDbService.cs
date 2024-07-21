using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBindGen;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Singularity.Contracts;
using Singularity.Models;

namespace Singularity.Services;

public class FirestoreDbService : IDatabaseService
{
    private Lazy<Task<JObjPtr>> FirestoreJSReference { get; }
    public IAuthenticatonService AuthService { get; }
    public ILogger<FirestoreDbService> Logger { get; }
    
    public FirestoreDbService(IJSRuntime runtime,IAuthenticatonService authService,ILogger<FirestoreDbService> logger)
    {
        AuthService = authService;
        Logger = logger;

        FirestoreJSReference = new(async () =>
        {
            await BindGen.InitAsync(runtime);
            Logger.LogInformation("creating firestore instance");
            return await BindGen.ImportRefAsync("/js/firebase.js");
        });

    }

    public async ValueTask<string?> PathToTableAsync(string tableName)
    {
        var user = await AuthService.GetLoggedInUserAsync();

        if (user == null)
        {
            Logger.LogError("user is not logged in");
            return null;
        }

        var path = "Users/"+user.Uid +"/UserData/"+ tableName;
        Logger.LogInformation("path:"+path);
        return path;
    }

    public ValueTask ConnectAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask CreateTableAsync<T>(string tableName, T table) where T : class
    {
        try
        {

            var path = await PathToTableAsync(tableName);
            if (path == null)
                return;

            var module = await FirestoreJSReference.Value;
            var ptr = await module.CallRefAwaitedAsync("FirestoreSetDoc", path, table);
            Logger.LogInformation("created table with name:" + tableName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant create table:" + tableName);
        }
    }

    public async ValueTask<IDbTable?> GetTableAsync(string tableName)
    {
        try
        {
            var path = await PathToTableAsync(tableName);
            if (path == null)
                return null;

            var module = await FirestoreJSReference.Value;
            var ptr = await module.CallRefAwaitedAsync("FirestoreGetDoc", path);
            Logger.LogInformation("got table with name:" + tableName);
            return new FirestoreDbTable(ptr,Logger,module,this);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant get table:" + tableName);
            return null;
        }
    }

    public async ValueTask DeleteTableAsync(string tableName)
    {
        try
        {

            var path = await PathToTableAsync(tableName);
            if (path == null)
                return;

            var module = await FirestoreJSReference.Value;
            var ptr = await module.CallRefAwaitedAsync("FirestoreDeleteDoc", path);
            Logger.LogInformation("deleted table with name:" + tableName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant delete table:" + tableName);
        }
    }

    public async ValueTask UpdateTableAsync<T>(string tableName,T tableData) where T : class
    {
        try
        {
            var path = await PathToTableAsync(tableName);
            if (path == null)
                return;

            var module = await FirestoreJSReference.Value;
            var ptr = await module.CallRefAwaitedAsync("FirestoreSetDoc", path, tableData);
            Logger.LogInformation("updated table with name:" + tableName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant update table:" + tableName);
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
