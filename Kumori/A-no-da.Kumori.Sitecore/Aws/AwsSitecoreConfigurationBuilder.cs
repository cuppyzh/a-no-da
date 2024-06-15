using A_no_da.Kumori.Core.Aws.SecretManager;
using Newtonsoft.Json;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace A_no_da.Kumori.Sitecore.Aws
{
    public class AwsSitecoreConfigurationBuilder: RuleBasedConfigReader
    {
        private const string PatchSourceAttributeName = "patch:sourceAwsSecretManager";
        private const string ValueAttributeName = "value";
        private static AwsSecretManagerServices _secretManagerServices = new AwsSecretManagerServices();

        protected override XmlDocument DoGetConfiguration()
        {
            var configuration = base.DoGetConfiguration();
            var configName = ConfigurationManager.AppSettings["A_no_da.Kumori.Sitecore.Aws.Secret.Name"];
            InjectEnvironmentVariables(
                variableNamePrefix: configName,
                key => configuration.SelectSingleNode($"/sitecore/settings/setting[{XPathCompareCaseInsensitive("name", key)}]"),
                key => ValueAttributeName,
                // Some Sitecore settings have their XML element inner text set instead of the "value" attribute, for example PublishingServiceUrlRoot.
                // In such cases we need to set the element inner text as well.
                setInnerTextWhenTargetNotFound: true);

            return configuration;
        }

        protected override void ReplaceGlobalVariables(XmlNode rootNode)
        {
            base.ReplaceGlobalVariables(rootNode);
        }

        private static IDictionary<string, string> GetEnvironmentVariables(string configName)
        {
            var typedDictionary = new Dictionary<string, string>();

            var secret = _secretManagerServices.GetAwsSecret(configName);

            if (string.IsNullOrEmpty(secret))
            {
                return typedDictionary;
            }

            return JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);
        }

        private static void InjectPatchSource(XmlNode node, string source)
        {
            var valueSourceAttribute = node.Attributes[PatchSourceAttributeName]
                ?? node.OwnerDocument.CreateAttribute(PatchSourceAttributeName, "http://www.sitecore.net/xmlconfig/");

            valueSourceAttribute.Value = string.IsNullOrEmpty(valueSourceAttribute.Value)
                ? source
                : valueSourceAttribute.Value += $", {source}";

            node.Attributes.Append(valueSourceAttribute);
        }

        private static string XPathCompareCaseInsensitive(string attributeName, string value)
        {
            return $"translate(@{attributeName}, 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ') = '{value.ToUpperInvariant()}'";
        }

        private void InjectEnvironmentVariables(string variableNamePrefix, Func<string, XmlNode> nodeSelector, Func<string, string> targetAttributeNameSelector, bool setInnerTextWhenTargetNotFound = false)
        {
            var environmentVariables = GetEnvironmentVariables(variableNamePrefix);

            foreach (var environmentVariable in environmentVariables)
            {
                var key = environmentVariable.Key;
                var node = nodeSelector(key);

                if (node == null)
                {
                    continue;
                }

                var targetAttributeName = targetAttributeNameSelector(key);
                var attributeFound = false;

                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (!attribute.Name.Equals(targetAttributeName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    attribute.Value = environmentVariable.Value;
                    attributeFound = true;
                    InjectPatchSource(node, environmentVariable.Key);
                }

                if (!attributeFound && setInnerTextWhenTargetNotFound)
                {
                    node.InnerText = environmentVariable.Value;
                    InjectPatchSource(node, environmentVariable.Key);
                }
            }
        }
    }
}
