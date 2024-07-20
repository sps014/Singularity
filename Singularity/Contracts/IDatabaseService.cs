using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IDatabaseService: IAsyncDisposable
{
    ValueTask ConnectAsync();
    ValueTask<IDbTable?> GetTableAsync(string tableName);
    ValueTask CreateTableAsync<T>(string tableName,T tableData) where T:class;
    ValueTask DeleteTableAsync(string tableName);
    ValueTask UpdateTableAsync<T>(string tableName, T tableData) where T:class;

}
