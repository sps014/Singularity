using Singularity.Components.Views;
using Singularity.Data;
using YoutubeExplode.Playlists;

namespace Singularity.Components.Pages;

public partial class UserPlaylistGroupPage
{
    private CreateNewPlaylistView? newPlaylistElement;
    private UserPlaylistView? PlaylistView;

    private string? playlistId;
    private bool DeleteBtnVisible = false;
    private bool CrossBtnVisible = false;
    private bool AddBtnVisible = true;

    private void OnPlusButtonClicked()
    {
        newPlaylistElement?.SetOpen();
        StateHasChanged();
    }
    private void CrossBtnClicked()
    {
        playlistId = null;
        DeleteBtnVisible = false;
        CrossBtnVisible = false;
        AddBtnVisible = true;
        PlaylistView?.ResetPlaylistSelected();
        StateHasChanged();
    }

    private async Task OnDeleteButtonClicked()
    {
        if(playlistId != null) 
            await PlaylistSettings.Current.RemovePlaylistAsync(playlistId);

        CrossBtnClicked();
    }
    private void PlaylistItemSelected(string playlistID)
    {
        playlistId = playlistID;
        DeleteBtnVisible = true;
        CrossBtnVisible = true;
        AddBtnVisible = false;
        StateHasChanged();
    }
}