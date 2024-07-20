using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IMusicHub
{
    ValueTask<ISong?> GetSongMetaDataAsync(string id);
    ValueTask<string?> GetSongStreamUrlAsync(string id);
    ValueTask<ICollection<string>> SuggestionsAsync(string query, CancellationTokenSource searchCancellation);
    IAsyncEnumerable<ISong> SearchAsync(string query, CancellationTokenSource searchCancellation);
    ValueTask<IMusicPlaylist?> GetPlaylistAsync(string id);
}
