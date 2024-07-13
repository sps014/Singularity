using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Models;

public record TabPage(string ImagePath, string Link,string Title)
{
    public bool IsActive { get; set; }
}