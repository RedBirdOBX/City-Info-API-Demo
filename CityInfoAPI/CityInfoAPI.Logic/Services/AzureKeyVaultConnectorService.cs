using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Services
{
    public class AzureKeyVaultConnector
    {
        private readonly IConfiguration _configuration;

        public AzureKeyVaultConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            // https://www.codeproject.com/Tips/1430794/Using-Csharp-NET-to-Read-and-Write-from-Azure-Key

            KeyVaultClient kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            SecretBundle secret = Task.Run(() => kvc.GetSecretAsync(_configuration["dsf-db-conn-secret"])).ConfigureAwait(false).GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(secret.Value))
            {
                throw new NullReferenceException("No value found for dsf-db-conn-secret.");
            }

            return secret.Value;
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(_configuration["city-api-app-reg"], _configuration["dsf-app-reg-client-secret"]);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);


            // dsf-app-reg-client-secret
            //0eWqsnLKn4Y@kyw=Kl=XguQzMUKXg05]


            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the Azure JWT token");
            }

            return result.AccessToken;
        }
    }
}
