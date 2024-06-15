using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_no_da.Kumori.Core.Aws.SecretManager
{
    public class AwsSecretManagerServices
    {
        public string GetAwsSecret(string key)
        {
            try
            {
                string secretName = key;

                var client = new AmazonSecretsManagerClient();
                var request = new GetSecretValueRequest
                {
                    SecretId = secretName
                };

                GetSecretValueResponse response;
                response = client.GetSecretValue(request);

                return response.SecretString;
            }
            catch
            {
                return null;
            }
        }

        public List<KeyValuePair<string, string>> GetAwsSecrets(string prefix)
        {
            try
            {
                var result = new List<KeyValuePair<string, string>>();
                var client = new AmazonSecretsManagerClient();
                var request = new ListSecretsRequest
                {
                    Filters = new List<Filter>() {
                    new Filter() {  Key = "name",
                                    Values = new List<string> { prefix }
                                }
                    }
                };
                var secrets = client.ListSecrets(request);

                if (secrets == null)
                {
                    return result;
                }

                foreach (var item in secrets.SecretList)
                {
                    var config = GetAwsSecret(item.Name);

                    if (!string.IsNullOrEmpty(config))
                    {
                        result.Add(new KeyValuePair<string, string>(item.Name, config));
                    }
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
