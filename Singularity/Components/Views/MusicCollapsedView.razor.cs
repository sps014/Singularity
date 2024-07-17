using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorBindGen;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Singularity.Services;

namespace Singularity.Components.Views;

public partial class MusicCollapsedView : IDisposable
{
#nullable disable
    [Inject]
    public AudioManager AudioManager { get; set; }
    [Inject]
    public IJSRuntime Runtime { get; set; }

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
        if ((DateTime.Now - _lastUpdateTime).TotalMilliseconds < 1000)
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


    public void Dispose()
    {
        AudioManager.MediaPlayer.StateChanged -= MediaPlayerStateChanged;
        AudioManager.MediaPlayer.PositionChanged -= MediaPlayerPositionChanged;
    }
}
