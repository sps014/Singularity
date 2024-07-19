using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Models;

public class AuthException : Exception
{
    public AuthException(string message) : base(message)
    {
        message = message.Replace("FirebaseError: Firebase: Error", string.Empty);
    }
}