using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Singularity.Contracts;
using Singularity.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Singularity.Services;

public class YoutubeMusicHub : IMusicHub
{
    private static YoutubeClient? youtubeClient = null;
    private const string SearchUrl = "https://clients1.google.com/complete/search?client=youtube&gs_ri=youtube&ds=yt&q=";
    private static HttpClient Http = new HttpClient();
    private Dictionary<string, string> cachedMediaUrls = new Dictionary<string, string>();

    public static YoutubeClient YoutubeClient
    {
        get
        {
            return youtubeClient ?? (youtubeClient = new YoutubeClient());
        }
    }

    public ILogger<YoutubeMusicHub> Logger { get; }

    public YoutubeMusicHub(ILogger<YoutubeMusicHub> logger)
    {
        Logger = logger;
    }

    public async ValueTask<ISong?> GetSongMetaDataAsync(string id)
    {
        ISong? song = null;

        await Task.Run(async () =>
        {
            try
            {
                var songInfo = await YoutubeClient.Videos.GetAsync(id);

                Logger.LogInformation($"fetched metadata for song   -> {songInfo.Id} -> {songInfo.Title}");

                song = new YouTubeSong(this)
                {
                    Description = songInfo.Description,
                    Duration = songInfo.Duration,
                    Id = songInfo.Id,
                    Name = songInfo.Title,
                    Singer = songInfo.Author.ChannelTitle,
                    ThumbnailUrl = songInfo.Thumbnails.GetWithHighestResolution().Url
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Can't get song with id :{id}");
                song = null;
            }
        });

        return song;

    }

    public async ValueTask<StreamUrl?> GetSongStreamUrlAsync(string id)
    {
        StreamUrl? songUrl = null;

        if (cachedMediaUrls.TryGetValue(id, out songUrl))
        {
            Logger.LogInformation($"read audio stream url from cache   -> {id}");
            return songUrl;
        }

        await Task.Run(async () =>
        {
            try
            {
                var songInfo = await YoutubeClient.Videos.Streams.GetManifestAsync(id);
                songUrl = songInfo.GetAudioOnlyStreams().GetWithHighestBitrate().Url;
                cachedMediaUrls[id] = songUrl;
                Logger.LogInformation($"read audio stream url from network   -> {id}");

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Can't get url of song with id :{id}");
                songUrl = null;
            }
        });

        return songUrl;
    }

    public async ValueTask<ICollection<string>> SuggestionsAsync(string query, CancellationTokenSource searchCancellation)
    {
        try
        {
            Logger.LogInformation($"Suggesting {query}");

            var original = query;
            query = Uri.EscapeDataString(query);
            query = SearchUrl + query;
            var res = await Http.GetAsync(query, searchCancellation.Token);
            var js = await res.Content.ReadAsStringAsync(searchCancellation.Token);

            var parts = js.Split('[').Where(t => t.Split('"').Length > 2).Select(t => t.Split('"')[1]);

            return parts.Distinct().ToList();
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "Cant suggest right now");
            return new List<string>();
        }
    }
    public async IAsyncEnumerable<ISong> SearchAsync(string query, CancellationTokenSource searchCancellation)
    {

        Logger.LogInformation($"showing result for {nameof(SearchAsync)}");

        await foreach (var video in YoutubeClient.Search.GetVideosAsync(query, searchCancellation.Token))
        {

            var song = new YouTubeSong(this)
            {
                Description = string.Empty,
                Duration = video.Duration,
                Id = video.Id,
                Name = video.Title,
                Singer = video.Author.ChannelTitle,
                ThumbnailUrl = video.Thumbnails.GetWithHighestResolution().Url
            };

            yield return song;
        }
    }

    public async ValueTask<IMusicPlaylist?> GetPlaylistAsync(string id)
    {
        try
        {
            var playlist = await YoutubeClient.Playlists.GetAsync(id);
            return new YoutubeMusicPlaylist(this)
            {
                Description = playlist.Description,
                Id = playlist.Id,
                Name = playlist.Title,
                ThumbnailUrl = playlist.Thumbnails.GetWithHighestResolution().Url,
                Singer = playlist.Author != null ? playlist.Author.ChannelTitle : string.Empty
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "cant fetch playlist metadata ->"+id);
            return null;
        }
    }
}
