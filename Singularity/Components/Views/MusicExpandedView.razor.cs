using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Singularity.Data;
using Singularity.Services;

namespace Singularity.Components.Views;

public partial class MusicExpandedView: IDisposable
{
#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
#nullable restore

    [Parameter]
    public EventCallback OnToggled { get; set; }
    private DateTime _lastUpdateTime = DateTime.MinValue;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AudioManager.MediaPlayer.StateChanged += MediaPlayerStateChanged;
        AudioManager.MediaPlayer.PositionChanged += MediaPlayerPositionChanged;
    }
    private async void MediaPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        if((DateTime.Now-_lastUpdateTime).TotalMilliseconds<1000)
        {
            return;
        }
        _lastUpdateTime = DateTime.Now;

        await InvokeAsync(() =>
        {
            StateHasChanged();

        });
    }

    private async void MediaPlayerStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        await InvokeAsync(async () =>
        {

            StateHasChanged();
            await Task.Delay(200);
        });
    }

    //On back button
    private void OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (MusicView.Instance == null)
            return;

        if (MusicView.Instance.IsExpanded)
        {
            MusicView.Instance.IsExpanded = false;
            context.PreventNavigation();
        }
    }

    private async Task HandlePlayPause()
    {
        if (AudioManager.MediaPlayer.CurrentState == MediaElementState.Playing)
        {
            AudioManager.Pause();
            StateHasChanged();
        }
        else
        {
            await AudioManager.PlayAsync();
            StateHasChanged();
        }
    }

    private async Task AddOrRemoveFromLiked()
    {
        if (AudioManager.CurrentSong == null)
            return;

        if(!UserSettings.Current.IsLiked(AudioManager.CurrentSong))
        {
            await UserSettings.Current.AddToLikeAsync(AudioManager.CurrentSong);
        }
        else
            await UserSettings.Current.RemoveFromLikeAsync(AudioManager.CurrentSong);

        StateHasChanged();
    }

    private void OnInputRangeSlider(ChangeEventArgs e)
    {
        AudioManager.MediaPlayer.SeekTo(TimeSpan.FromSeconds(double.Parse(e.Value.ToString()!)));
    }

    public void Dispose()
    {
        AudioManager.MediaPlayer.StateChanged -= MediaPlayerStateChanged;
        AudioManager.MediaPlayer.PositionChanged -= MediaPlayerPositionChanged;
    }

}
