using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_no_da.Kumori.Core.Aws.Models
{
    public class AwsSecretManagerResponseModel
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "engine")]
        public string Engine { get; set; }

        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }

        [JsonProperty(PropertyName = "dbname")]
        public string DatabaseName { get; set; }

        [JsonProperty(PropertyName = "dbInstanceIdentifier")]
        public string DatabaseInstanceIdentifier { get; set; }

        [JsonProperty(PropertyName = "usePort")]
        public string UsePort { get; set; }

        public bool UsingPort()
        {
            if (!string.IsNullOrEmpty(UsePort) && UsePort.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public class AwsSecretManagerKeyValueResponse
        {
            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
        }

        public static AwsSecretManagerResponseModel ParseFromConfig(Dictionary<string, string> config)
        {
            var model = new AwsSecretManagerResponseModel();

            config.TryGetValue("username", out string username);
            model.Username = username;

            config.TryGetValue("password", out string password);
            model.Password = password;

            config.TryGetValue("engine", out string engine);
            model.Engine = engine;

            config.TryGetValue("host", out string host);
            model.Host = host;

            config.TryGetValue("dbname", out string dbname);
            model.DatabaseName = dbname;

            config.TryGetValue("dbInstanceIdentifier", out string dbInstanceIdentifier);
            model.DatabaseInstanceIdentifier = dbInstanceIdentifier;

            config.TryGetValue("usePort", out string usePort);
            model.UsePort = usePort;

            return model;
        }
    }
}
