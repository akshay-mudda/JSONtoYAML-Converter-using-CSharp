using System;
using System.IO;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using System.Configuration;

namespace JSONtoYAML_Converter_using_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Read source and destination paths from the app.config file
                string sourceDirectory = ConfigurationManager.AppSettings["SourceDirectory"];
                string destinationDirectory = ConfigurationManager.AppSettings["DestinationDirectory"];

                // Ensure the destination directory exists
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Get all JSON files from the source directory
                string[] jsonFiles = Directory.GetFiles(sourceDirectory, "*.json");

                if (jsonFiles.Length == 0)
                {
                    Console.WriteLine("No JSON files found in the source directory.");
                    return;
                }

                foreach (string jsonFilePath in jsonFiles)
                {
                    try
                    {
                        // Get the file name without extension
                        string fileName = Path.GetFileNameWithoutExtension(jsonFilePath);

                        // Set the output YAML file path
                        string outputFilePath = Path.Combine(destinationDirectory, fileName + ".yaml");

                        // Convert JSON to YAML
                        var jsonData = File.ReadAllText(jsonFilePath);
                        var yamlData = ConvertJsonToYaml(jsonData);

                        // Write YAML file
                        File.WriteAllText(outputFilePath, yamlData);

                        Console.WriteLine($"Successfully converted '{jsonFilePath}' to YAML.");

                        // Optionally, delete the source JSON file after conversion
                        File.Delete(jsonFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting file {jsonFilePath}: {ex.Message}");
                    }
                }

                Console.WriteLine("Conversion process completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Helper method to convert JSON string to YAML string
        public static string ConvertJsonToYaml(string jsonData)
        {
            // Deserialize the JSON into an ExpandoObject for dynamic handling
            var deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(jsonData, new ExpandoObjectConverter());

            // Serialize the object into YAML format using YamlDotNet
            var serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(deserializedObject);

            return yaml;
        }
    }
}