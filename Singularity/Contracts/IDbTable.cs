using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IDbTable
{
    ValueTask SetValue<T>(string columnName, T value);
    ValueTask<T> GetValue<T>(string columnName);
    ValueTask<bool> DeleteValue(string columnName);
    ValueTask<T> ToAsync<T>();
}
