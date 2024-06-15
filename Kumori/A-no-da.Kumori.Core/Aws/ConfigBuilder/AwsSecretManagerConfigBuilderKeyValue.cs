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
    public class AwsSecretManagerConfigBuilderKeyValue : KeyValueConfigBuilder
    {
        private readonly AwsSecretManagerServices _awsSecretManagerServices = new AwsSecretManagerServices();
        private const string _keyPrefix = "source:";

        public override ICollection<KeyValuePair<string, string>> GetAllValues(string prefix)
        {
            return _GetConfigs();
        }

        public override string GetValue(string key)
        {
            var configs = _GetConfigs();

            key = key.Replace(KeyPrefix, "");
            if (configs == null || !configs.TryGetValue(key, out string value))
            {
                return null;
            }

            return value;
        }

        private Dictionary<string, string> _GetConfigs()
        {
            if (!KeyPrefix.StartsWith(_keyPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var configSource = KeyPrefix.Replace(_keyPrefix, "");
            var secret = _awsSecretManagerServices.GetAwsSecret(configSource);

            if (secret == null)
            {
                return null;
            }

            var keyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(secret);

            if (keyValuePair == null)
            {
                return null;
            }

            return keyValuePair;
        }
    }
}