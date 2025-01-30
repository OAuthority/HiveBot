using System.Reflection;
using IniParser;
using IniParser.Model;

namespace HiveBot.Core;

public class Configuration
{
    /// <summary>
    /// Quick wrapper to get configuration variables from the ini file; this should be
    /// used only for stuff like Discord tokens, database credentials etc.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static string GetConfigVariable(string section, string variable)
    {
        // Get the directory of the executing assembly (the same folder as the DLL is executing in when in prod)
        string assemblyLocation = Assembly.GetEntryAssembly()?.Location;
        if (string.IsNullOrEmpty(assemblyLocation))
        {
            // Fallback for edge cases
            assemblyLocation = Assembly.GetExecutingAssembly().Location;
        }
        
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        string configPath = Path.Combine(assemblyDirectory, "config.ini");
        
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        FileIniDataParser parser = new();
        IniData data = parser.ReadFile(configPath);
        
        if (data.Sections.ContainsSection(section))
        {
            var keyData = data[section];
            if (keyData.ContainsKey(variable))
            {
                return keyData[variable];
            }
        }

        return null;
    }
}
