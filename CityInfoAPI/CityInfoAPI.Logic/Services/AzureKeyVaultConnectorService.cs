using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Services
{
    public class AzureKeyVaultConnector
    {
        private readonly IConfiguration _configuration;

        // inject the IConfiguration service and store it in a field
        public AzureKeyVaultConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            // go authenticate this request
            KeyVaultClient kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

            // get the secret's value - in this case, the connection string
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

            // get the city api app reg ID, shown in step 5 of docs AND also get the secret for the app in step 6
            ClientCredential clientCred = new ClientCredential(_configuration["city-api-app-reg"], _configuration["dsf-app-reg-client-secret"]);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the Azure JWT token");
            }

            return result.AccessToken;
        }
    }
}
