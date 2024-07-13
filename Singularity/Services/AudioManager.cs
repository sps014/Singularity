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

namespace Singularity.Services;


public class AudioManager
{
    private static MediaElement? MediaElement;
    private List<ISong> queuedSongs { get; } = new List<ISong>();
    private bool playAtleastOnce = false;


    public IReadOnlyList<ISong> QueuedSongs => queuedSongs;

    public ISong? CurrentSong => QueuedSongs.FirstOrDefault();

    public LoopMode LoopMode { get; set; } = LoopMode.All;

    public MediaElement MediaPlayer =>MediaElement!;

    public ILogger<AudioManager> Logger { get; }

    public float MediaPositionPercent
    {
        get
        {
            if(CurrentSong == null)
                return 0;
            return (float)(MediaPlayer.Position.TotalMilliseconds * 100.0f / MediaPlayer.Duration.TotalMilliseconds);
        }
    }


    public AudioManager(ILogger<AudioManager> logger)
    {
        Logger = logger;
    }

    public async ValueTask AddSongAsync(ISong song)
    {
        var sameSong = QueuedSongs.FirstOrDefault(x => x.Id == song.Id);
        if (sameSong!=null)
        {
            queuedSongs.Remove(sameSong);
            queuedSongs.Insert(0, song);
            await MediaPlayer.SeekTo(TimeSpan.Zero);
            Logger.LogInformation($"{song.Id} -> {song.Name} already in queue");
            return;
        }

        Logger.LogInformation($"{song.Id}  -> {song.Name} added in queue");
        queuedSongs.Add(song);
    }

    public async ValueTask PlayPreviousSongAsync()
    {
        if (QueuedSongs.Count <= 0)
            return;

        var time = MediaPlayer.Position;

        //if played more than 5 sec than play same from start
        if (time.TotalSeconds > 5)
        {
            await MediaPlayer.SeekTo(TimeSpan.Zero);
            await PlayAsync();
            return;
        }

        //remove last song and bring it front and play
        var last = QueuedSongs.Last();
        queuedSongs.Remove(last);
        await AddSongAsync(last);

        await MediaPlayer.SeekTo(TimeSpan.Zero);
        await PlayAsync();
    }

    public async ValueTask PlayNextSongAsync()
    {
        if (QueuedSongs.Count <= 0)
            return;

        //move current song to end
        var first = CurrentSong!;
        queuedSongs.Remove(CurrentSong!);
        queuedSongs.Add(first);

        await MediaPlayer.SeekTo(TimeSpan.Zero);
        await PlayAsync();

    }
    public async ValueTask PlayAsync()
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

        MediaPlayer.Source = MediaSource.FromUri(url);
        MediaPlayer.Play();

        Logger.LogInformation($"started playing {CurrentSong.Id} -> {CurrentSong.Name}");
    }

    internal static void InitMediaElement(MediaElement mediaElement)
    {
        MediaElement = mediaElement;
    }
    internal void SetupEvents()
    {
        Logger.LogInformation($"Initialized Media Player and setting up events");
        MediaPlayer.MediaEnded += MediaPlayerMediaEnded;
    }

    private async void MediaPlayerMediaEnded(object? sender, EventArgs e)
    {
        Logger.LogInformation($"Media has ended {CurrentSong?.Id} -> {CurrentSong?.Name}");

        switch(LoopMode)
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
    }
}

public enum LoopMode
{
    All,
    None,
    Same
}