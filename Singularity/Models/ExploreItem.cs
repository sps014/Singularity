using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Singularity.Models;

public class ExploreItem
{
    [JsonConstructor]
    public ExploreItem(
        string Name,
        string PlaylistId,
        string Thumbnail
    )
    {
        this.Name = Name;
        this.PlaylistId = PlaylistId;
        this.Thumbnail = Thumbnail;
    }

    public string Name { get; }
    public string PlaylistId { get; }
    public string Thumbnail { get; }

    public static async Task<ExploreItem[]?> LoadAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("wwwroot/json/explore_playlist.json");
        using var stringreader = new StreamReader(stream);
        var json = stringreader.ReadToEnd();
        return JsonSerializer.Deserialize<ExploreItem[]>(json);
    }
}