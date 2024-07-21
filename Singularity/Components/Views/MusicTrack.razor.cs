using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Singularity.Contracts;
using Singularity.Services;

namespace Singularity.Components.Views;

public partial class MusicTrack:IDisposable
{
    [Parameter]
    public string? CurrentSelectedSongInList { get; set; }

#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
#nullable restore

    [Parameter]
    public ISong? Song { get; set; }

    [Parameter]
    public string? SongId { get; set; }

    [Parameter]
    public bool CanPlay { get; set; } = true;

    private bool IsPlaying = false;
    private bool isSongLoading = false;

    private string inactiveColor = "background-color:rgba(117,132,147,0.4);margin-top:4px;";

    private string BackgroundColor()
    {
        if (IsPlaying || (CurrentSelectedSongInList!=null && CurrentSelectedSongInList == Song.Id))
            return "rgba(192, 182, 230, 0.2)";
        else
            return inactiveColor;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AudioManager.MediaPlayer.StateChanged += MediaStateChanged;
    }
    protected override async Task OnInitializedAsync()
    {
        if (Song == null && SongId != null)
            Song = await MusicHub.GetSongMetaDataAsync(SongId);

        UpdateIsPlaying();
        StateHasChanged();

        await base.OnInitializedAsync();
    }
    private async void MediaStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        await InvokeAsync(() =>
        {
            UpdateIsPlaying();
            StateHasChanged();
        });
    }

    void UpdateIsPlaying()
    {
        IsPlaying =
            AudioManager.MediaPlayer.CurrentState==MediaElementState.Playing
            && AudioManager.CurrentSong != null
            && Song!=null && Song.Id == AudioManager.CurrentSong.Id;
    }

    private async void AddAndPlayAsync()
    {
        if (Song==null || !CanPlay)
        {
            return;
        }

        isSongLoading = true;
        StateHasChanged();

        await AudioManager.PlayNowAsync(Song);

        isSongLoading = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        AudioManager.MediaPlayer.StateChanged -= MediaStateChanged;
    }
}
