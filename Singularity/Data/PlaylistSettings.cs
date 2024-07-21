using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Singularity.Contracts;
using Singularity.Models;

namespace Singularity.Data;

internal partial class PlaylistSettings : ObservableObject
{
    [ObservableProperty] Dictionary<string, UserPlaylist> playlists = new Dictionary<string, UserPlaylist>();

#pragma warning disable CS0612 // Type or member is obsolete
    private static PlaylistSettings current = new PlaylistSettings();
#pragma warning restore CS0612 // Type or member is obsolete

    private static bool loadedFromDb = false;

    public static PlaylistSettings Current => current;

    [Obsolete]
    public PlaylistSettings()
    {
    }

    public static async ValueTask LoadSettingsFromDb(IDatabaseService databaseService)
    {
        UserSettings.DataBaseService = databaseService;

        var table = await databaseService.GetTableAsync(nameof(PlaylistSettings));

        if (table == null) return;

        var online = await table.ToAsync<PlaylistSettings>();
        if (online == null) return;

        current = online;
        loadedFromDb = true;
    }

    public static ValueTask SaveSettingsInDb(IDatabaseService databaseService)
    {
        if (!loadedFromDb)
            return ValueTask.CompletedTask;

        return databaseService.UpdateTableAsync(nameof(PlaylistSettings), current);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (UserSettings.DataBaseService == null)
            return;

        base.OnPropertyChanged(e);
        await SaveSettingsInDb(UserSettings.DataBaseService);
    }

    public bool IsPlaylistExist(string playlistName)
    {
        return Playlists.ContainsKey(playlistName);
    }

    public ValueTask CreateNewPlaylistAsync(string playlistName)
    {
        if (IsPlaylistExist(playlistName))
            return ValueTask.CompletedTask;

        Playlists[playlistName] = new UserPlaylist()
        {
            Id = playlistName,
            Name = playlistName,
            Singer = string.Empty,
            Description = string.Empty,
            ThumbnailUrl = string.Empty
        };

        PlaylistUpdated?.Invoke(this, EventArgs.Empty);

        return SaveSettingsInDb(UserSettings.DataBaseService!);
    }

    public ValueTask AddSongToPlaylistAsync(string playlistName, ISong song)
    {
        if (Playlists[playlistName].Songs.Contains(song.Id))
            return ValueTask.CompletedTask;

        Playlists[playlistName].Songs.Add(song.Id);
        PlaylistUpdated?.Invoke(this, EventArgs.Empty);
        return SaveSettingsInDb(UserSettings.DataBaseService!);
    }

    public ValueTask RemoveSongFromPlaylistAsync(string playlistName, ISong song)
    {
        Playlists[playlistName].Songs.Remove(song.Id);
        PlaylistUpdated?.Invoke(this, EventArgs.Empty);
        return SaveSettingsInDb(UserSettings.DataBaseService!);
    }
    public ValueTask RemoveSongFromPlaylistAsync(string playlistName, string song)
    {
        Playlists[playlistName].Songs.Remove(song);
        PlaylistUpdated?.Invoke(this, EventArgs.Empty);
        return SaveSettingsInDb(UserSettings.DataBaseService!);
    }
    public ValueTask RemovePlaylistAsync(string playlistName)
    {
        Playlists.Remove(playlistName);
        PlaylistUpdated?.Invoke(this, EventArgs.Empty);
        return SaveSettingsInDb(UserSettings.DataBaseService!);
    }

    public event EventHandler? PlaylistUpdated;
}