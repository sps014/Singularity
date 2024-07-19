using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Singularity.Models;

#nullable disable
public class FirebaseUser
{
    [JsonPropertyName("user")]
    public FirebaseLoggedUser User { get; set; }

    [JsonPropertyName("providerId")]
    public object ProviderId { get; set; }

    [JsonPropertyName("operationType")]
    public string OperationType { get; set; }
}


public class FirebaseLoggedUser
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("isAnonymous")]
    public bool IsAnonymous { get; set; }

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("lastLoginAt")]
    public string LastLoginAt { get; set; }

    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; }

    [JsonPropertyName("appName")]
    public string AppName { get; set; }
}

#nullable disable