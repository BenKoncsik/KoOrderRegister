using KORCore.Modules.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Services
{
    public interface ILocalDatabase: IDatabaseModel
    {
        Task<int> CreateOrUpdateDatabaseConnection(ConnectionDeviceData connection);
        Task<int> DeleteConnection(ConnectionDeviceData connection);
        Task<ConnectionDeviceData> GetConnectionById(Guid id);
        IAsyncEnumerable<ConnectionDeviceData> GetConnectionDataAsStreamAsync(CancellationToken cancellationToken);
        IAsyncEnumerable<ConnectionDeviceData> SearchConnectionDataAsStreamAsync(string searchValue, CancellationToken cancellationToken);
    }
}
