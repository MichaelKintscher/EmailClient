using Domain.Common;
using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InterfaceAdapters.Json
{
    public static class ServiceProviderAccountAdapter
    {
        public static string Serialize(List<ServiceProviderAccount> accounts)
        {
            return String.Empty;
        }

        public static List<ServiceProviderAccount> Deserialize(string lines)
        {
            JsonNode? jsonObject = JsonNode.Parse(lines);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize Service Provider Account from was not in the expected format.");
            }

            JsonArray accountsArray;
            try
            {
                accountsArray = jsonObject["accounts"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // For each account in the array...
            List<ServiceProviderAccount> accounts = new List<ServiceProviderAccount>();
            foreach (var accountJsonValue in accountsArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string accountJson = accountJsonValue.ToJsonString();

                // Parse the account data.
                ServiceProviderAccount? account = JsonSerializer.Deserialize<ServiceProviderAccount>(accountJson);

                // Add a new hidden calendar record to the list.
                accounts.Add(account);
            }

            return accounts;
        }
    }
}
