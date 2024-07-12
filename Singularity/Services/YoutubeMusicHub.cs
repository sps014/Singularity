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
using YoutubeExplode.Videos.Streams;

namespace Singularity.Services;

public class YoutubeMusicHub : IMusicHub
{
    private static YoutubeClient? youtubeClient = null;

    private Dictionary<string,string> cachedMediaUrls = new Dictionary<string,string>();
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
        ISong? song =null;

        await Task.Run(async() =>
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
}
