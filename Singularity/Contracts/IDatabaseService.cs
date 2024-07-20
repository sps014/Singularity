using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IDatabaseService
{
    ValueTask ConnectAsync();
    ValueTask<IDbTable> GetTableAsync(string tableName);
    ValueTask<IDbTable> CreateTableAsync<T>(string tableName,T table) where T:class;
}
