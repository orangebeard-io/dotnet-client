using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Orangebeard.Client.OrangebeardProperties.PropertyNames;

namespace Orangebeard.Client.OrangebeardProperties
{
    public class OrangebeardConfiguration
    {
        public string Endpoint { get; private set; }
        public string AccessToken { get; private set; }
        public string ProjectName { get; private set; }
        public string TestSetName { get; private set; }
        public string Description { get; private set; }
        public string ListenerIdentification { get; private set; }
        public ISet<Entities.Attribute> Attributes { get; private set; }
        public IList<string> FileUploadPatterns { get; private set; }

        public OrangebeardConfiguration(string propertyFile)
        {
            ReadPropertyFile(propertyFile);
            ReadEnvironmentVariables(".");
            ReadEnvironmentVariables("_");
            if(!RequiredPropertiesArePresent())
            {
                throw new OrangebeardConfigurationException("Not all required configuration properties (Endpoint, AccessToken, ProjectName, TestSetName) are present!");
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
            Attributes = new HashSet<Entities.Attribute>(
                config.GetKeyValues(
                    ConfigurationPath.Attributes, 
                    new HashSet<KeyValuePair<string, string>>()
                )
                .Select(a => new Entities.Attribute(a.Key, a.Value))
                //.ToList()
            );
            if (!RequiredPropertiesArePresent())
            {
                throw new OrangebeardConfigurationException("Not all required configuration properties (Endpoint, AccessToken, ProjectName, TestSetName) are present!");
            }
            ProjectName = ProjectName.ToLower();
        }

        public OrangebeardConfiguration()
        {
            ReadPropertyFile(ORANGEBEARD_PROPERTY_FILE);
            ReadEnvironmentVariables(".");
            ReadEnvironmentVariables("_");
            ProjectName = ProjectName.ToLower();
        }

        public OrangebeardConfiguration(string endpoint, Guid uuid, string projectName, string testSetName)
        {
            this.Endpoint = endpoint;
            this.AccessToken = uuid.ToString();
            this.ProjectName = projectName.ToLower();
            this.TestSetName = testSetName;
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
            if (Environment.GetEnvironmentVariable(ORANGEBEARD_FILEUPLOAD_PATTERNS.Replace(".", separator)) != null)
            {
                string patternList = Environment.GetEnvironmentVariable(ORANGEBEARD_FILEUPLOAD_PATTERNS.Replace(".", separator));
                FileUploadPatterns = new List<string>(patternList.Split(';')); 
            }
        }

        private void ReadPropertyFile(string propertyFile)
        {
            IDictionary<string, string> properties;
            try
            {
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

                var uploadPatterns = GetValueOrNull(properties, ORANGEBEARD_FILEUPLOAD_PATTERNS);
                if (uploadPatterns != null)
                {
                    FileUploadPatterns = new List<string>(uploadPatterns.Split(';'));
                }
                else
                {
                    FileUploadPatterns = new List<string>();
                }


            } catch (FileNotFoundException)
            {
                //ignore
            }
        }

        private ISet<Entities.Attribute> ExtractAttributes(string attributeList)
        {
            ISet<Entities.Attribute> attributes = new HashSet<Entities.Attribute>();
            if (attributeList == null)
            {
                return attributes;
            }

            foreach (string attribute in attributeList.Split(';'))
            {
                if (attribute.Contains(":"))
                {
                    string[] keyVal = attribute.Split(':');
                    attributes.Add(new Entities.Attribute(key: keyVal[0].Trim(), value: keyVal[1].Trim()));
                }
                else
                {
                    attributes.Add(new Entities.Attribute(value: attribute));
                }
            }

            return attributes;
        }


        private string GetValueOrNull(IDictionary<string, string> dict, string key)
        {
            dict.TryGetValue(key, out string result);
            return result;
        }
    }

}
