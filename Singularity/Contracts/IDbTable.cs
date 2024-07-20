using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IDbTable
{
    ValueTask<string?> GetIdAsync();
    ValueTask<bool> SetValueAsync<T>(string columnName, T value);
    ValueTask<T?> GetValueAsync<T>(string columnName);
    ValueTask<bool> DeleteValueAsync(string columnName);
    ValueTask<T?> ToAsync<T>();
}
