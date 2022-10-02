using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config
{
    public interface IStorageProvider
    {
        public Task SaveConnectionDataAsync(string tokenFileName, Dictionary<string, OAuthToken> tokens);

        public Task<Dictionary<string, OAuthToken>> TryLoadTokenDataAsync(string tokenFileName);
    }
}
