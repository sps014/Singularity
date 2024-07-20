using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorBindGen;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Singularity.Contracts;
using Singularity.Services;

namespace Singularity.Models;

public class FirestoreDbTable : IDbTable
{
    public JObjPtr Ptr { get; }
    public ILogger<FirestoreDbService> Logger { get; }
    public JObjPtr FirestoreJSReference { get; }
    public FirestoreDbService FirebaseDbService { get; }

    public FirestoreDbTable(JObjPtr ptr, ILogger<FirestoreDbService> logger,JObjPtr firestoreJSReference, FirestoreDbService firebaseDbService)
    {
        Ptr = ptr;
        Logger = logger;
        FirestoreJSReference = firestoreJSReference;
        FirebaseDbService = firebaseDbService;
    }

    public async ValueTask<bool> DeleteValueAsync(string columnName)
    {
        try
        {
            var path = await FirebaseDbService.PathToTableAsync((await GetIdAsync())!);
            await FirestoreJSReference.CallVoidAwaitedAsync("FirestoreDeleteField", path!,columnName);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Can't Delete {columnName} for : " + await GetIdAsync());
            return false;
        }
    }

    public async ValueTask<T?> GetValueAsync<T>(string columnName)
    {
        try
        {
            var data = await ToAsync<Dictionary<string, JsonElement>>();
            if (data == null)
            {
                Logger.LogInformation("reading field data is null for column: "+columnName);
                return default;
            }
            Logger.LogInformation("reading the data for column: "+columnName);

            if (data.ContainsKey(columnName))
                return data[columnName].Deserialize<T?>();

            return default;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant read:" + columnName);
            return default;
        }
    }

    public async ValueTask<bool> SetValueAsync<T>(string columnName, T value)
    {
        try
        {
            var path = await FirebaseDbService.PathToTableAsync((await GetIdAsync())!);
            await FirestoreJSReference.CallVoidAwaitedAsync("FirestoreUpdateDoc",path!,
                new Dictionary<string, object?>() { [columnName] = value });
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Can't update {columnName} for : "+ await GetIdAsync());
            return false;
        }
    }

    public ValueTask<T?> ToAsync<T>()
    {
        try
        {
            return Ptr.CallAsync<T?>("data");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Can't get data for");
            return ValueTask.FromResult<T?>(default(T));
        }
    }

    public ValueTask<string?> GetIdAsync()
    {
        try
        {
            return Ptr.PropValAsync<string?>("id");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Can't get id");
            return ValueTask.FromResult<string?>(null);
        }
    }
}
