using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orangebeard.Client.V3.OrangebeardConfig.Providers
{
    /// <summary>
    /// Finds files in a directory and consider their content as a value for configuration properties.
    /// </summary>
    public class DirectoryProbingConfigurationProvider : IConfigurationProvider
    {
        private readonly string _directoryPath;
        private readonly string _prefix;
        private readonly string _delimiter;
        private readonly bool _optional;

        /// <summary>
        /// Creates new instance of <see cref="DirectoryProbingConfigurationProvider"/> class.
        /// </summary>
        /// <param name="directoryPath">Path to a directory where to find files.</param>
        /// <param name="prefix">Limit files searching.</param>
        /// <param name="delimiter">Consider this string as hierarchic property.</param>
        /// <param name="optional">Returns empty list of properties if directory doesn't exist.</param>
        public DirectoryProbingConfigurationProvider(string directoryPath, string prefix, string delimiter,
            bool optional)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            _delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
            _optional = optional;
        }

        /// <inheritdoc />
        public IDictionary<string, string> Load()
        {
            var properties = new Dictionary<string, string>();

            if (Directory.Exists(_directoryPath))
            {
                var directory = new DirectoryInfo(_directoryPath);

                var escapedDelimiter = Regex.Escape(_delimiter);
                var pattern = $"{_prefix.ToLowerInvariant()}({escapedDelimiter}[a-zA-Z]+)+";

                var ignoredFileExtensions = new[] { ".exe", ".dll", ".pdb", ".log" };

                var candidates = directory.EnumerateFiles().Where(f =>
                    Regex.IsMatch(f.Name.ToLowerInvariant(), pattern) &&
                    !ignoredFileExtensions.Contains(f.Extension.ToLowerInvariant()));

                foreach (var candidate in candidates)
                {
                    var key = candidate.Name.ToLowerInvariant()
                        .Replace($"{_prefix.ToLowerInvariant()}{_delimiter}", string.Empty)
                        .Replace(_delimiter, ConfigurationPath.KeyDelimeter);
                    var value = File.ReadAllText(candidate.FullName);

                    properties[key] = value.Trim();
                }
            }
            else
            {
                if (!_optional)
                {
                    throw new DirectoryNotFoundException(
                        $"Required directory not found by '{_directoryPath}' path as configuration provider.");
                }
            }

            return properties;
        }
    }
}