using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Data;
using Singularity.Models;
using YoutubeExplode.Playlists;

namespace Singularity.Components.Views;

public partial class UserPlaylistView : IDisposable
{
    [Parameter]
    public bool IsInAddMode { get; set; } = false;

    [Parameter]
    public EventCallback<string> OnPlaylistSelected { get; set; }
    private string? playlistId;
    protected override void OnInitialized()
    {
        PlaylistSettings.Current.PlaylistUpdated += CurrentPlaylistUpdated;
    }
    private string GetStyle(UserPlaylist userPlaylist)
    {
        if (userPlaylist.Id == playlistId)
        {
            return "rgba(7, 17, 27, 0.6)";
        }
        return "rgba(7, 17, 27, 0.2)";
    }
    private void CurrentPlaylistUpdated(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void PlaylistSelected(string id)
    {
        if (IsInAddMode)
        {
            playlistId = id;
            OnPlaylistSelected.InvokeAsync(id);
            StateHasChanged();
            return;
        }

        //playlistId is not null if in longselect mode
        if (playlistId!=null || id == null)
            return;

       

        Nav.NavigateTo("/userPlaylistExpandedPage/" + id);
    }
    public void ResetPlaylistSelected()
    {
        playlistId = null;
        StateHasChanged();
    }

    private void OnLongPressed(UserPlaylist playlist)
    {
        if (IsInAddMode)
            return;

        playlistId = playlist.Id;
        OnPlaylistSelected.InvokeAsync(playlist.Id);
        StateHasChanged();
    }

    public void Dispose()
    {
        PlaylistSettings.Current.PlaylistUpdated -= CurrentPlaylistUpdated;
    }

}
