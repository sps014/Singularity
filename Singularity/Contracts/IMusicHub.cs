using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Contracts;

public interface IMusicHub
{
    ValueTask<ISong?> GetSongMetaData(string id);
    ValueTask<string?> GetSongStreamUrl(string id);

}
