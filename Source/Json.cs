using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace WPlugZ_CLI.Source
{

    public class JSON
    {

        protected JsonNode rootNode;

        /// <summary>
        /// Loads the JSON from a file
        /// </summary>
        /// <param name="filepath">The file to load the JSON from</param>
        public void LoadFromFile(string filepath)
        {

            var jsonData = File.ReadAllText(filepath);
            rootNode = JsonNode.Parse(jsonData);

        }

        /// <summary>
        /// Loads the JSON from a string
        /// </summary>
        /// <param name="jsonAsString">The string to convert to JSON</param>
        public void Load(string jsonAsString)
        {

            rootNode = JsonNode.Parse(jsonAsString);

        }

        /// <summary>
        /// Saves the JSON onto a file
        /// </summary>
        /// <param name="filepath">The file to save the JSON in</param>
        public void Save(string filepath)
        {

            File.WriteAllText(filepath, ToJsonString()); // I love them indented JSON

        }

        /// <summary>
        /// Returns a key's value.
        /// </summary>
        /// <typeparam name="T">The type of the key's value</typeparam>
        /// <param name="key">The key to access</param>
        /// <returns>The value of the key</returns>
        public T Get<T>(string key)
        {

            return rootNode[key].GetValue<T>();

        }

        /// <summary>
        /// Sets a key's value.
        /// </summary>
        /// <param name="key">The key whose value you wish to modify</param>
        /// <param name="value">The new value to give to the key</param>
        public void Set(string key, object value)
        {

            rootNode[key] = JsonValue.Create(value);

        }

        /// <summary>
        /// Returns the JSON from root as a string.
        /// </summary>
        /// <param name="indentJson">Whether to toggle pretty printing on(=default) or off</param>
        /// <returns>The JSON object from root, as a string</returns>
        public string ToJsonString(bool indentJson = true)
        {

            return rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = indentJson });

        }

    }

};

