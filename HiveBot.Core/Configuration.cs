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
    public static string GetConfigVariable( string section, string variable )
    {
        FileIniDataParser parser = new();
        IniData data = parser.ReadFile("config.ini");
        
        // check if the section exists, if it doesn't we can't do anything, obviously
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
