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
    private DateTime lastUpdateTime = DateTime.MinValue;
    private AddToPlaylistView? addToPlaylistView;


    protected override void OnInitialized()
    {
        base.OnInitialized();
        AudioManager.MediaPlayer.StateChanged += MediaPlayerStateChanged;
        AudioManager.MediaPlayer.PositionChanged += MediaPlayerPositionChanged;
    }
    private async void MediaPlayerPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        if((DateTime.Now-lastUpdateTime).TotalMilliseconds<1000)
        {
            return;
        }
        lastUpdateTime = DateTime.Now;

        await InvokeAsync(() =>
        {
            StateHasChanged();

        });
    }

    private void ShowAddToPlaylist()
    {
        addToPlaylistView?.Show();
    }

    private async void MediaPlayerStateChanged(object? sender, MediaStateChangedEventArgs e)
    {
        await InvokeAsync(async () =>
        {

            StateHasChanged();
            await Task.Delay(200);
        });
    }

    private string GetLoopImage()
    {
        if (UserSettings.Current.LoopMode == Models.LoopMode.All)
            return "./images/repeat_all.png";
        else if (UserSettings.Current.LoopMode == Models.LoopMode.Same)
            return "./images/repeat_same.png";
        else
            return "./images/repeat_none.png";
    }

    private void ChangeLoopMode()
    {
        switch(UserSettings.Current.LoopMode)
        {
            case Models.LoopMode.All:
                UserSettings.Current.LoopMode = Models.LoopMode.Same;
                break;
            case Models.LoopMode.Same:
                UserSettings.Current.LoopMode = Models.LoopMode.None;
                break;
            case Models.LoopMode.None:
                UserSettings.Current.LoopMode = Models.LoopMode.All;
                break;
        }
        StateHasChanged();
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
        if(e.Value==null)
            return;
        
        AudioManager.MediaPlayer.SeekTo(TimeSpan.FromSeconds(double.Parse(e.Value.ToString()!)));
    }

    public void Dispose()
    {
        AudioManager.MediaPlayer.StateChanged -= MediaPlayerStateChanged;
        AudioManager.MediaPlayer.PositionChanged -= MediaPlayerPositionChanged;
    }

}
