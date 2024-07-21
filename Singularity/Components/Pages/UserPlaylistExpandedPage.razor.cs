using Microsoft.AspNetCore.Components;
using Singularity.Components.Views;
using Singularity.Contracts;
using Singularity.Data;
using Singularity.Models;
using YoutubeExplode.Playlists;

namespace Singularity.Components.Pages;

public partial class UserPlaylistExpandedPage
{
    [Parameter]
    public string? Id { get; set; }
    private UserPlaylist? musicPlaylist;
    private IAsyncEnumerable<ISong>? songs;

    private bool DeleteBtnVisible = false;
    private bool CrossBtnVisible = false;

    private SmartMusicListView? smartMusicListView;
    private string? selectedSongId;
    protected override void OnInitialized()
    {
        if (Id == null)
            return;

        var playlist = PlaylistSettings.Current.Playlists
            .TryGetValue(Id, out musicPlaylist);

        if (musicPlaylist != null)
            songs = GetSongsAsync();
    }

    private void OnLongPress(string songid)
    {
        selectedSongId = songid;
        DeleteBtnVisible = true;
        CrossBtnVisible = true;
        StateHasChanged();
    }
    private void CrossBtnClicked()
    {
        DeleteBtnVisible = false;
        CrossBtnVisible = false;
        smartMusicListView?.ResetCurrentSelectionInList();
        selectedSongId = null;
        StateHasChanged();
    }

    private async Task OnDeleteButtonClicked()
    {
        if (selectedSongId != null && Id != null)
        {
            await PlaylistSettings.Current.RemoveSongFromPlaylistAsync(Id, selectedSongId);
            smartMusicListView?.RemoveSongFromList(selectedSongId);
        }
          CrossBtnClicked();
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
