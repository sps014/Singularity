using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Contracts;

namespace Singularity.Models;

public class User : IUser
{
    public string Uid { get; }
    public User(string uid)
    {
        Uid = uid;
    }

}
