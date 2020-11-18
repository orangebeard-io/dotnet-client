﻿using System.Collections.Generic;

namespace ReportPortal.Shared.Configuration
{
    /// <summary>
    /// Stores configuration variables from different providers.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Fetched configuration variables.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Returns value of configuration property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        T GetValue<T>(string property);

        /// <summary>
        /// Returns value of configuration property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetValue<T>(string property, T defaultValue);

        /// <summary>
        /// Returns values of configuration property.
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetValues<T>(string property);

        /// <summary>
        /// Returns values of configuration property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetValues<T>(string property, IEnumerable<T> defaultValue);

        /// <summary>
        /// Returns pairs of key:value of configuration property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, T>> GetKeyValues<T>(string property);

        /// <summary>
        /// Returns pairs of key:value of configuration property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, T>> GetKeyValues<T>(string property, IEnumerable<KeyValuePair<string, T>> defaultValue);
    }
}
