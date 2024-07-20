using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IMusicPlaylist
{
    string Id { get; set; }
    string Name { get; set; }
    string Singer { get; set; }
    string Description { get; set; }
    string ThumbnailUrl { get; set; }
    PlaylistType Type { get; set; }
    IAsyncEnumerable<ISong> GetAsync();
}

public enum PlaylistType
{
    Online,
    UserMade
}