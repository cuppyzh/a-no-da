using A_no_da.Kumori.Core.Aws.Models;
using A_no_da.Kumori.Core.Aws.SecretManager;
using Microsoft.Configuration.ConfigurationBuilders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_no_da.Kumori.Core.Aws.ConfigBuilder
{
    public class AwsSecretManagerConfigBuilder: KeyValueConfigBuilder
    {
        private readonly AwsSecretManagerServices _awsSecretManagerServices = new AwsSecretManagerServices();

        public AwsSecretManagerConfigBuilder() { }

        public override ICollection<KeyValuePair<string, string>> GetAllValues(string prefix)
        {
            var result = new List<KeyValuePair<string, string>>();
            var secrets = _awsSecretManagerServices.GetAwsSecrets(prefix);

            if (secrets == null)
            {
                return result;
            }

            foreach (var secret in secrets)
            {
                var config = _ParseConfig(secret.Value);

                if (!string.IsNullOrEmpty(config))
                {
                    result.Add(new KeyValuePair<string, string>(secret.Key.Replace(prefix, ""), config));
                }
            }

            return result;
        }

        public override string GetValue(string key)
        {
            var secret = _awsSecretManagerServices.GetAwsSecret(key);
            return _ParseConfig(secret);
        }

        private string _ParseConfig(string secret)
        {
            var keyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(secret);

            if (keyValuePair == null)
            {
                return null;
            }

            if (keyValuePair.TryGetValue("value", out string value))
            {
                return value;
            }

            var databaseModel = AwsSecretManagerResponseModel.ParseFromConfig(keyValuePair);

            if (databaseModel.Engine.Equals("sqlServer", StringComparison.InvariantCultureIgnoreCase))
            {
                return _GetSqlServerConnectionString(databaseModel);
            }

            return null;
        }

        private string _GetSqlServerConnectionString(AwsSecretManagerResponseModel model)
        {
            if (model.UsingPort())
            {
                return $"Data source={model.Host}:{model.Port};" +
                    $"Initial Catalog={model.DatabaseName};" +
                    $"User Id={model.Username};" +
                    $"Password={model.Password}";
            }

            return $"Data source={model.Host};" +
                $"Initial Catalog={model.DatabaseName};" +
                $"User Id={model.Username};" +
                $"Password={model.Password}";
        }
    }
}
