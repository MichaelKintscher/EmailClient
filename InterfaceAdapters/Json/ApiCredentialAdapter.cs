using Domain.Common;
using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InterfaceAdapters.Json
{
    public static class ApiCredentialAdapter
    {
        public static ApiCredential Deserialize(string credentials)
        {
            JsonNode? credentialsJson = JsonNode.Parse(credentials);
            if (credentialsJson == null)
            {
                throw new JsonFormatException("JSON to deserialize API Credential from was not in the expected format.");
            }

            string clientId;
            string clientSecret;
            try
            {
                credentialsJson = credentialsJson["installed"].AsObject();
                clientId = credentialsJson["client_id"].GetValue<string>();
                clientSecret = credentialsJson["client_secret"].GetValue<string>();
            }
            catch (NullReferenceException ex)
            {
                throw new JsonFormatException("JSON to deserialize API Credential from was not in the expected format.");
            }

            return new ApiCredential()
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }
    }
}
