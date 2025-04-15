using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            rootNode = node ?? new JsonObject();

        }

        /// <summary>
        /// Loads the JSON from a file
        /// </summary>
        /// <param name="filepath">The file to load the JSON from</param>
        public JSON LoadFromFile(string filepath)
        {

            var jsonData = File.ReadAllText(filepath);
            rootNode = JsonNode.Parse(jsonData);
            return this;

        }

        /// <summary>
        /// Loads the JSON from a string
        /// </summary>
        /// <param name="jsonAsString">The string to convert to JSON</param>
        public JSON Load(string jsonAsString)
        {

            rootNode = JsonNode.Parse(jsonAsString);
            return this;

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
        /// <typeparam name="T">The datatype to convert the value to when returning</typeparam>
        /// <param name="key">The key matching the value</param>
        /// <param name="defaultValue">A default value in case the key does not exist</param>
        /// <returns>The value, as a readable data type</returns>
        public T Get<T>(string key, T defaultValue = default)
        {

            if (rootNode is not JsonObject jsonObject) return defaultValue;

            var value = jsonObject[key];
            if (value == null) return defaultValue;
            return ConvertJsonNodeToReadableType<T>(value);

        }

        /// <summary>
        /// Sets a key's value. If it doesn't exist, it gets created.
        /// </summary>
        /// <param name="key">The key to assign a value to</param>
        /// <param name="value">The value to assign</param>
        /// <returns>The current state of the class</returns>
        /// <exception cref="InvalidOperationException">The root node is not a JSON object</exception>
        public JSON Set(string key, object value)
        {

            if (rootNode is not JsonObject jsonObject) throw new InvalidOperationException("The root node is not a JSON object.");

            jsonObject[key] = (JsonNode)ConvertReadableTypeToJsonNode(value);
            return this;

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
        /// Converts a JSON Node to a clear and readable data type.
        /// </summary>
        /// <typeparam name="T">Datatype to convert to</typeparam>
        /// <param name="node">The Json element to convert to</param>
        /// <returns>A converted value in the specified data type</returns>
        /// <exception cref="InvalidOperationException">Unsupported data type</exception>
        public T ConvertJsonNodeToReadableType<T>(JsonNode node)
        {

            return (T)(node switch
            {

                JsonObject jsonObject => new JSON(jsonObject),
                JsonArray jsonArray => jsonArray.Select(arrayItem => ConvertJsonNodeToReadableType<object>(arrayItem)).ToArray(),
                JsonValue jsonValue => ConvertJsonValueToType<T>(jsonValue),
                _ => throw new InvalidOperationException($"Unsupported type: {node?.GetType()}")

            });

        }

        private object ConvertJsonValueToType<T>(JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out JsonElement element))
            {
                return element.ValueKind switch
                {
                    JsonValueKind.String => element.GetString(),
                    JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => element
                };
            }

            throw new InvalidOperationException("Failed to convert JsonValue");
        }

        /// <summary>
        /// Converts a readable C# type to a JSON Node.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted JsonNode/JsonObject/JsonArray/JsonValue.</returns>
        /// <exception cref="InvalidOperationException">Unsupported type</exception>
        public object ConvertReadableTypeToJsonNode(object value)
        {

            return value switch
            {

                Dictionary<string, object> dictionary => new JsonObject(
                    dictionary.ToDictionary(
                        pair => pair.Key,
                        pair => (JsonNode) ConvertReadableTypeToJsonNode(pair.Value)
                    )
                ),

                IDictionary<string, object> dictionary => new JsonObject(
                    dictionary.ToDictionary(
                        pair => pair.Key,
                        pair => (JsonNode) ConvertReadableTypeToJsonNode(pair.Value)
                    )
                ),

                IEnumerable<object> enumerableObject => new JsonArray(
                    enumerableObject.Select(item => (JsonNode) ConvertReadableTypeToJsonNode(item)).ToArray()
                ),

                string strValue => JsonValue.Create(strValue),
                int integer => JsonValue.Create(integer),
                double doubleValue => JsonValue.Create(doubleValue),
                bool boolean => JsonValue.Create(boolean),
                JSON json => json.AsNode,
                null => null,
                _ when Nullable.GetUnderlyingType(value.GetType()) != null => JsonValue.Create(value),
                _ => throw new InvalidOperationException($"Unsupported type: {value.GetType()}")

            };

        }

        /// <summary>
        /// Enumerates KeyValue pairs for the whole JSON object, while converting the data types to JsonNodes.
        /// </summary>
        /// <returns>Key Value pair for each iteration.</returns>
        /// <exception cref="InvalidOperationException">The root node is not a JSON object.</exception>
        public IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs()
        {

            if (rootNode is JsonObject jsonObject)
            {

                foreach (var pair in jsonObject)
                {

                    yield return KeyValuePair.Create(pair.Key, ConvertJsonNodeToReadableType<object>(pair.Value));
                
                }

            }

            else
            {
                throw new InvalidOperationException("The root node is not a JSON object.");
            }

        }

        /// <summary>
        /// Allows for easier access to the inner JsonNode, in a readonly way.
        /// </summary>
        /// <returns>The readonly JsonNode</returns>
        public JsonNode AsNode
        {
            get { return rootNode; }
        }

        /// <summary>
        /// Checks if the JsonObject contains a certain key.
        /// </summary>
        /// <param name="keyName">The key to check</param>
        /// <returns>Whether the key is part of the JsonObject or not</returns>
        /// <exception cref="InvalidOperationException">The root node is not a JSON object</exception>
        public bool Contains(string keyName)
        {

            if (rootNode is JsonObject jsonObject)
            {

                return jsonObject.ContainsKey(keyName);

            }

            else
            {
                throw new InvalidOperationException("The root node is not a JSON object.");
            }

        }

        /// <summary>
        /// Less optimized than Length, but makes sure no erros are raised.
        /// </summary>
        /// <returns>The length or -1 (the latter occurs if the rootNode is not a JsonObject)</returns>
        public int GetLengthSafely()
        {

            if (rootNode is JsonObject jsonObject)
            {

                return jsonObject.Count;

            }

            else
            {
                return -1;
            }

        }

        /// <summary>
        /// Amount of elements in the JsonObject.
        /// If the rootNode is not a JsonObject, an error is called.
        /// </summary>
        public int Length
        {
            get
            {
                var jsonObject = (JsonObject)rootNode;
                return jsonObject.Count; 
            }
        }

        /// <summary>
        /// Amount of elements in the JsonObject.
        /// 
        /// This is a wrapper for GetLengthSafely().
        /// If you want maximum optimization, use the direct property (Length) instead of this one.
        /// </summary>
        public int Count
        {
            get { return GetLengthSafely(); }
        }

    }

};

