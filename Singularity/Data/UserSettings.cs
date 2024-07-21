using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Singularity.Contracts;
using Singularity.Models;

namespace Singularity.Data;

public partial class UserSettings : ObservableObject
{

    [ObservableProperty]
    LoopMode loopMode = LoopMode.All;

    [ObservableProperty]
    ObservableCollection<string> likedSongs = new ObservableCollection<string>();

#pragma warning disable CS0612 // Type or member is obsolete
    private static UserSettings current = new UserSettings();
#pragma warning restore CS0612 // Type or member is obsolete


    public static UserSettings Current => current;

    internal static IDatabaseService? DataBaseService;
    private static bool loadedFromDb = false;

    [Obsolete]
    public UserSettings()
    {

    }

    public static async ValueTask LoadSettingsFromDb(IDatabaseService databaseService)
    {
        DataBaseService = databaseService;

        var table = await databaseService.GetTableAsync(nameof(UserSettings));

        if (table == null) return;

        var online = await table.ToAsync<UserSettings>();
        if (online == null)  return;
        
        current = online;
        loadedFromDb = true;
    }

    public static ValueTask SaveSettingsInDb(IDatabaseService databaseService)
    {
        if (!loadedFromDb)
            return ValueTask.CompletedTask;

        return databaseService.UpdateTableAsync(nameof(UserSettings),current);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (DataBaseService == null)
            return;

        base.OnPropertyChanged(e);
        await SaveSettingsInDb(DataBaseService);
    }

    public async ValueTask AddToLikeAsync(ISong song)
    {
        if (DataBaseService==null || IsLiked(song)) return;

        LikedSongs.Insert(0,song.Id);
        await SaveSettingsInDb(DataBaseService);
    }
    public async ValueTask RemoveFromLikeAsync(ISong song)
    {
        if (DataBaseService == null || !IsLiked(song)) return;

        LikedSongs.Remove(song.Id);
        await SaveSettingsInDb(DataBaseService);
    }

    public bool IsLiked(ISong song)
    {
        return LikedSongs.Contains(song.Id);
    }

    public async IAsyncEnumerable<ISong> GetLikedSongAsync(IMusicHub musicHub)
    {
        foreach (var songId in LikedSongs.Reverse())
        {
           var song = await musicHub.GetSongMetaDataAsync(songId);
            if (song == null) continue;
            yield return song;
        }
    }
}
