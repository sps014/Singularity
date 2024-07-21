using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Data;

namespace Singularity.Components.Views;

public partial class AddToPlaylistView
{
    private bool isActive = false;
    private string? currentPalylistId;
    CreateNewPlaylistView? newPlaylistElement;

    private void NewAddToPlaylistButtonClicked()
    {
        newPlaylistElement?.SetOpen();
    }
    private async Task AddToPlaylist()
    {
        if (AudioManager.CurrentSong == null || string.IsNullOrWhiteSpace(currentPalylistId))
        {
            OnClose();
            return;
        }

        await PlaylistSettings.Current.AddSongToPlaylistAsync(currentPalylistId, AudioManager.CurrentSong);
        OnClose();
    }

    private Task OnNewPlaylistCreated(string id)
    {
        currentPalylistId = id;
        return AddToPlaylist();
    }
    private void PlaylistSelected(string id)
    {
        currentPalylistId = id;
        StateHasChanged();
    }
    private void OnClose()
    {
        isActive = false;
        StateHasChanged();
    }
    public void Show()
    {
        isActive = true;
        StateHasChanged();
    }
}