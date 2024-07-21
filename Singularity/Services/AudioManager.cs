using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using Singularity.Contracts;
using Singularity.Data;
using Singularity.Models;

namespace Singularity.Services;


public class AudioManager(ILogger<AudioManager> logger) : BindableObject
{
    private static MediaElement? _mediaElement;
    private List<ISong> QueuedSongs { get; } = [];
    private bool playAtleastOnce = false;

    
    public ISong? CurrentSong => QueuedSongs.FirstOrDefault();

    public MediaElement MediaPlayer =>_mediaElement!;

    public ILogger<AudioManager> Logger { get; } = logger;

    public float MediaPositionPercent
    {
        get
        {
            if(CurrentSong == null)
                return 0;
            return (float)(MediaPlayer.Position.TotalMilliseconds * 100.0f / MediaPlayer.Duration.TotalMilliseconds);
        }
    }


    private void SetMetaData()
    {
        if(CurrentSong == null)
            return;

        MediaPlayer.MetadataTitle = CurrentSong.Name;
        MediaPlayer.MetadataArtist = CurrentSong.Singer;
        MediaPlayer.MetadataArtworkUrl = CurrentSong.ThumbnailUrl;
    }

    public async ValueTask AddSongAsync(ISong song)
    {
        await this.Dispatcher.DispatchAsync(async() =>
        {
            var sameSong = QueuedSongs.FirstOrDefault(x => x.Id == song.Id);
            if (sameSong != null)
            {
                QueuedSongs.Remove(sameSong);
                QueuedSongs.Insert(0, song);
                await MediaPlayer.SeekTo(TimeSpan.Zero);
                Logger.LogInformation($"{song.Id} -> {song.Name} already in queue");
                return;
            }

            Logger.LogInformation($"{song.Id}  -> {song.Name} added in queue");
            QueuedSongs.Add(song);
        });
       
    }

    public async ValueTask PlayPreviousSongAsync()
    {
        if (QueuedSongs.Count <= 0)
            return;

        var time = MediaPlayer.Position;

        //if played more than 5 sec than play same from start
        if (time.TotalSeconds > 5)
        {
            await PlayAsync(true);
            return;
        }

        //remove last song and bring it front and play
        var last = QueuedSongs.Last();
        QueuedSongs.Remove(last);
        await AddSongAsync(last);

        await PlayAsync(true);
    }

    public async ValueTask PlayNextSongAsync()
    {

        if (QueuedSongs.Count <= 0)
            return;

        //move current song to end
        var first = CurrentSong!;
        QueuedSongs.Remove(CurrentSong!);
        QueuedSongs.Add(first);

        await PlayAsync(seekToStart:true);

    }

    public async ValueTask PlayNowAsync(ISong song)
    {
        Pause();
        var existingItem = QueuedSongs.FirstOrDefault(x=>x.Id==song.Id);
        var first = QueuedSongs.FirstOrDefault();

        if(first != null)
        {
            QueuedSongs.Remove(first);
            QueuedSongs.Add(first);
        }

        if(existingItem == null)
        {
            QueuedSongs.Insert(0, song);
            await PlayAsync(true);
            return;
        }

        //move current song to end
        QueuedSongs.Remove(existingItem);
        QueuedSongs.Insert(0,existingItem);

        await PlayAsync(seekToStart: true);

    }
    public async ValueTask PlayAsync(bool seekToStart =false)
    {
        if (CurrentSong is null)
            return;

        if (!playAtleastOnce)
        {
            playAtleastOnce = true;
            SetupEvents();
        }

        var url = await CurrentSong.GetAudioUrlAsync();

        if(url==null)
            return;

        if (MediaPlayer.Source==null ||
            (MediaPlayer.Source is UriMediaSource source 
             && source.Uri!=null && source.Uri.ToString()!=url))
        {
            MediaPlayer.Source = MediaSource.FromUri(url);
        }

        await this.Dispatcher.DispatchAsync(async () =>
        {
            SetMetaData();

            MediaPlayer.Play();

            if (seekToStart)
                await MediaPlayer.SeekTo(TimeSpan.Zero);
        });


        Logger.LogInformation($"started playing {CurrentSong.Id} -> {CurrentSong.Name}");
    }

    internal static void InitMediaElement(MediaElement mediaElement)
    {
        _mediaElement = mediaElement;
    }
    internal void SetupEvents()
    {
        Logger.LogInformation($"Initialized Media Player and setting up events");
        MediaPlayer.MediaEnded += MediaPlayerMediaEnded;
    }

    private async void MediaPlayerMediaEnded(object? sender, EventArgs e)
    {
        await this.Dispatcher.DispatchAsync(async() =>
        {
            Logger.LogInformation($"Media has ended {CurrentSong?.Id} -> {CurrentSong?.Name}");

            switch (UserSettings.Current.LoopMode)
            {
                case LoopMode.Same:
                    await MediaPlayer.SeekTo(TimeSpan.Zero);
                    await PlayAsync();
                    break;
                case LoopMode.All:
                    await PlayNextSongAsync();
                    break;
                default:
                    break;
            }
        });
        
    }

    public void Pause()
    {
        if (CurrentSong is null) return;
        Logger.LogInformation($"Paused {CurrentSong.Id}");
        MediaPlayer.Pause();
    }
}
