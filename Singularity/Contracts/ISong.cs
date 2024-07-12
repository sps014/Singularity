using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface ISong:IEquatable<ISong>
{
    string Id { get; set; }
    string Name { get; set; }
    string Singer { get; set; }
    string Description { get; set; }
    string ThumbnailUrl { get; set; }
    TimeSpan? Duration { get; set; }
    IMusicHub MusicHub { get; init; }

    ValueTask<StreamUrl?> GetAudioUrlAsync();
}
