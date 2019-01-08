using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        PluginImporter importer = AssetImporter.GetAtPath("Assets/NuitrackSDK/Nuitrack/NuitrackAssembly/nuitrack.net.dll") as PluginImporter;
        importer.SetCompatibleWithAnyPlatform(true);
        Debug.Log("nuitrack.net.dll platform \"Any Platform\"");
    }
}
