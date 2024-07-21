using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Singularity.Contracts;

namespace Singularity.Models;

public class UserPlaylist : IMusicPlaylist
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Singer { get; set; }
    public required string Description { get; set; }
    public required string ThumbnailUrl { get; set; }
    public PlaylistType Type { get; set; } = PlaylistType.UserMade;

    public List<string> Songs { get; set; } = new List<string>();

    public UserPlaylist()
    {
    }

    [Obsolete]
    public async IAsyncEnumerable<ISong> GetAsync()
    {
        await Task.CompletedTask;
        yield break;
    }
}
