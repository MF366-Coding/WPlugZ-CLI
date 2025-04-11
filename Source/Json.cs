using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace WPlugZ_CLI.Source
{

    public class JSON
    {

        protected JsonNode rootNode;

        /// <summary>
        /// Initializes a JSON object.
        /// </summary>
        /// <param name="node">The node to start with (defaults to null)</param>
        public JSON(JsonNode node = null)
        {

            if (node != null)
            {

                rootNode = node;

            }

        }

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
        /// <exception cref="NullReferenceException">Attempting to access a key that does not exist</exception>
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

        /// <summary>
        /// Sets a key's value to be a nested JSON.
        /// </summary>
        /// <param name="key">The key whose value you wish to change</param>
        /// <param name="innerJson">The JSON you wish to "nest"</param>
        public void SetNestedJson(string key, JSON innerJson)
        {

            rootNode[key] = innerJson.AsNode;

        }

        /// <summary>
        /// Returns a key's value as a JSON object if possible.
        /// </summary>
        /// <param name="key">The key whose value must be returned</param>
        /// <returns>The inner JSON present in the key</returns>
        /// <exception cref="NullReferenceException">The key does not exist or is not a valid JSON object.</exception>
        public JSON GetNestedJson(string key)
        {
            
            var innerNode = rootNode[key] as JsonObject ?? throw new NullReferenceException($"The key '{key}' does not exist or does not represent a JSON object.");
            JSON innerNodeAsJson = new(innerNode);
            return innerNodeAsJson;

        }


        /// <summary>
        /// Allows for easier access to the inner JsonNode, in a readonly way.
        /// </summary>
        /// <returns>The readonly JsonNode</returns>
        public JsonNode AsNode
        {
            get { return rootNode; }
        }

    }

};

