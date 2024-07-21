using Microsoft.AspNetCore.Components;
using Singularity.Contracts;
using Singularity.Data;
using Singularity.Models;

namespace Singularity.Components.Pages;

public partial class UserPlaylistExpandedPage
{
    [Parameter]
    public string? Id { get; set; }
    private UserPlaylist? musicPlaylist;
    private IAsyncEnumerable<ISong>? songs;
    protected override void OnInitialized()
    {
        if (Id == null)
            return;

        var playlist = PlaylistSettings.Current.Playlists
            .TryGetValue(Id, out musicPlaylist);

        if (musicPlaylist != null)
            songs = GetSongsAsync();
    }

    private async IAsyncEnumerable<ISong> GetSongsAsync()
    {
        if (musicPlaylist == null)
        {
            yield break;
        }
        foreach (var songId in musicPlaylist.Songs)
        {
            var song = await MusicHub.GetSongMetaDataAsync(songId);

            if (song != null)
                yield return song;
        }
    }
}
