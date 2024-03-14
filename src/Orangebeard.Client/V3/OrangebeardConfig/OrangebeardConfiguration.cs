using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using static Orangebeard.Client.V3.OrangebeardConfig.PropertyNames;
using Attribute = Orangebeard.Client.V3.Entity.Attribute;

namespace Orangebeard.Client.V3.OrangebeardConfig
{
    public class OrangebeardConfiguration
    {
        public string Endpoint { get; private set; }
        public string AccessToken { get; private set; }
        public string ProjectName { get; private set; }
        public string TestSetName { get; private set; }
        public string Description { get; private set; }
        public string ListenerIdentification { get; private set; }
        public ISet<Attribute> Attributes { get; private set; }
        public IList<string> FileUploadPatterns { get; private set; }

        public OrangebeardConfiguration(string propertyFile)
        {
            ReadPropertyFile(propertyFile);
            ReadEnvironmentVariables(".");
            ReadEnvironmentVariables("_");
            if (!RequiredPropertiesArePresent())
            {
                throw new OrangebeardConfigurationException(
                    "Not all required configuration properties (Endpoint, AccessToken, ProjectName, TestSetName) are present!");
            }

            ProjectName = ProjectName.ToLower();
        }

        public OrangebeardConfiguration(IConfiguration config)
        {
            Endpoint = config.GetValue<string>(ConfigurationPath.ServerUrl);
            AccessToken = config.GetValue<string>(ConfigurationPath.ServerAuthenticationUuid);
            ProjectName = config.GetValue<string>(ConfigurationPath.ServerProject);
            TestSetName = config.GetValue<string>(ConfigurationPath.TestSetName);
            Description = config.GetValue<string>(ConfigurationPath.TestSetDescription);
            Attributes = new HashSet<Attribute>(config
                .GetKeyValues(ConfigurationPath.Attributes, new HashSet<KeyValuePair<string, string>>())
                .Select(a => new Attribute { Key = a.Key, Value = a.Value }).ToList());
            if (!RequiredPropertiesArePresent())
            {
                throw new OrangebeardConfigurationException(
                    "Not all required configuration properties (Endpoint, AccessToken, ProjectName, TestSetName) are present!");
            }

            ProjectName = ProjectName.ToLower();
        }

        public OrangebeardConfiguration()
        {
            FileUploadPatterns = new List<string>();
            ReadPropertyFile(ORANGEBEARD_PROPERTY_FILE);
            ReadJsonConfigFile(ORANGEBEARD_JSON_CONFIG_FILE);
            ReadEnvironmentVariables(".");
            ReadEnvironmentVariables("_");
            ProjectName = ProjectName.ToLower();
        }

        public OrangebeardConfiguration WithListenerIdentification(string ListenerIdentification)
        {
            this.ListenerIdentification = ListenerIdentification;
            return this;
        }

        public bool RequiredPropertiesArePresent()
        {
            return Endpoint != null && AccessToken != null && ProjectName != null && TestSetName != null;
        }

        private void ReadEnvironmentVariables(string separator)
        {
            if (Environment.GetEnvironmentVariable(ORANGEBEARD_ENDPOINT.Replace(".", separator)) != null)
            {
                Endpoint = Environment.GetEnvironmentVariable(ORANGEBEARD_ENDPOINT.Replace(".", separator));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_ACCESS_TOKEN.Replace(".", separator)) != null)
            {
                AccessToken = Environment.GetEnvironmentVariable(ORANGEBEARD_ACCESS_TOKEN.Replace(".", separator));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_PROJECT.Replace(".", separator)) != null)
            {
                ProjectName = Environment.GetEnvironmentVariable(ORANGEBEARD_PROJECT.Replace(".", separator));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_TESTSET.Replace(".", separator)) != null)
            {
                TestSetName = Environment.GetEnvironmentVariable(ORANGEBEARD_TESTSET.Replace(".", separator));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_DESCRIPTION.Replace(".", separator)) != null)
            {
                Description = Environment.GetEnvironmentVariable(ORANGEBEARD_DESCRIPTION.Replace(".", separator));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_ATTRIBUTES.Replace(".", separator)) != null)
            {
                Attributes =
                    ExtractAttributes(
                        Environment.GetEnvironmentVariable(ORANGEBEARD_ATTRIBUTES.Replace(".", separator)));
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_REF_URL.Replace(".", separator)) != null)
            {
                var refUrlAttribute = new Attribute
                {
                    Key = "reference_url",
                    Value = Environment.GetEnvironmentVariable(ORANGEBEARD_REF_URL.Replace(".", separator))
                };

                if (Attributes == null)
                {
                    Attributes = new HashSet<Attribute> { refUrlAttribute };
                }
                else
                {
                    Attributes.Add(refUrlAttribute);
                }
            }

            if (Environment.GetEnvironmentVariable(ORANGEBEARD_FILEUPLOAD_PATTERNS.Replace(".", separator)) != null)
            {
                string patternList =
                    Environment.GetEnvironmentVariable(ORANGEBEARD_FILEUPLOAD_PATTERNS.Replace(".", separator));
                FileUploadPatterns = new List<string>(patternList.Split(';'));
            }
        }

        private void ReadJsonConfigFile(string jsonFileName)
        {
            var jsonProperties = GetJsonConfig(jsonFileName);
            if (jsonProperties.Count == 0) return;
            foreach (var property in jsonProperties)
            {
                switch (property.Key)
                {
                    case ORANGEBEARD_ENDPOINT:
                        Endpoint = property.Value.ToString();
                        break;
                    case ORANGEBEARD_ACCESS_TOKEN:
                        AccessToken = property.Value.ToString();
                        break;
                    case ORANGEBEARD_PROJECT:
                        ProjectName = property.Value.ToString();
                        break;
                    case ORANGEBEARD_TESTSET:
                        TestSetName = property.Value.ToString();
                        break;
                    case ORANGEBEARD_DESCRIPTION:
                        Description = property.Value.ToString();
                        break;
                    case ORANGEBEARD_ATTRIBUTES:
                        GetAttributesFromJsonArray((JArray)property.Value);
                        break;
                    case ORANGEBEARD_FILEUPLOAD_PATTERNS:
                        ((List<string>)FileUploadPatterns).AddRange(
                            ((JArray)property.Value).ToObject<List<string>>());
                        break;
                    case ORANGEBEARD_REF_URL:
                        Attributes.Add(new Attribute { Key = "reference_url", Value = property.Value.ToString() });
                        break;
                }
            }
        }

        private static Dictionary<string, object> GetJsonConfig(string jsonFileName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            while (currentDir != null)
            {
                var filePath = Path.Combine(currentDir, jsonFileName);
                if (File.Exists(filePath))
                {
                    try
                    {
                        var content = File.ReadAllText(filePath);
                        var jsonObject = JObject.Parse(content);
                        return jsonObject.ToObject<Dictionary<string, object>>();
                    }
                    catch (IOException)
                    {
                        return new Dictionary<string, object>();
                    }
                }

                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            return new Dictionary<string, object>();
        }

        private void ReadPropertyFile(string propertyFile)
        {
            try
            {
                IDictionary<string, string> properties;
                using (TextReader reader = new StreamReader(propertyFile))
                {
                    properties = PropertyFileLoader.Load(reader);
                }

                Endpoint = GetValueOrNull(properties, ORANGEBEARD_ENDPOINT);
                AccessToken = GetValueOrNull(properties, ORANGEBEARD_ACCESS_TOKEN);
                ProjectName = GetValueOrNull(properties, ORANGEBEARD_PROJECT);
                TestSetName = GetValueOrNull(properties, ORANGEBEARD_TESTSET);
                Description = GetValueOrNull(properties, ORANGEBEARD_DESCRIPTION);
                Attributes = ExtractAttributes(GetValueOrNull(properties, ORANGEBEARD_ATTRIBUTES));
                FileUploadPatterns =
                    new List<string>(GetValueOrNull(properties, ORANGEBEARD_FILEUPLOAD_PATTERNS).Split(';'));

                if (GetValueOrNull(properties, ORANGEBEARD_REF_URL) != null)
                {
                    Attributes.Add(new Attribute
                        { Key = "reference_url", Value = GetValueOrNull(properties, ORANGEBEARD_REF_URL) });
                }
            }
            catch (FileNotFoundException)
            {
                //ignore
            }
        }

        private ISet<Attribute> ExtractAttributes(string attributeList)
        {
            ISet<Attribute> attributes = new HashSet<Attribute>();
            if (attributeList == null)
            {
                return attributes;
            }

            foreach (string attribute in attributeList.Split(';'))
            {
                if (attribute.Contains(":"))
                {
                    string[] keyVal = attribute.Split(':');
                    attributes.Add(new Attribute { Key = keyVal[0].Trim(), Value = keyVal[1].Trim() });
                }
                else
                {
                    attributes.Add(new Attribute { Value = attribute });
                }
            }

            return attributes;
        }


        private static string GetValueOrNull(IDictionary<string, string> dict, string key)
        {
            dict.TryGetValue(key, out var result);
            return result;
        }

        private void GetAttributesFromJsonArray(JArray attrs)
        {
            foreach (var attr in attrs)
            {
                var attribute = (JObject)attr;
                if (attribute.Value<string>("key") != null)
                {
                    Attributes.Add(new Attribute(attribute.Value<string>("key"),
                        attribute.Value<string>("value")));
                }
                else
                {
                    Attributes.Add(new Attribute(attribute.Value<string>("value")));
                }
            }
        }
    }
}