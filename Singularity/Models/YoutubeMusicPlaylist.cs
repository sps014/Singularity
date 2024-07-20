using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Contracts;
using Singularity.Services;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;

namespace Singularity.Models;

internal class YoutubeMusicPlaylist : IMusicPlaylist
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ThumbnailUrl { get; set; }
    public required string Id { get; set; }
    public required string Singer { get; set; }
    public YoutubeMusicHub YoutubeMusicHub { get; }

    public PlaylistType Type { get; set; } = PlaylistType.Online;

    public YoutubeMusicPlaylist(YoutubeMusicHub youtubeMusicHub)
    {
        YoutubeMusicHub = youtubeMusicHub;
    }

    public async IAsyncEnumerable<ISong> GetAsync()
    {
        await foreach(var music in YoutubeMusicHub.YoutubeClient.Playlists.GetVideosAsync(Id))
        {
            yield return new YouTubeSong(YoutubeMusicHub)
            {
                Description = string.Empty,
                Duration = music.Duration,
                Id = music.Id,
                Name = music.Title,
                Singer =music.Author.ChannelTitle,
                ThumbnailUrl = music.Thumbnails.GetWithHighestResolution().Url
            };
        }
    }
}
