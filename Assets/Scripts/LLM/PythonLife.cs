using Python.Runtime;
using System;
using UnityEngine;

public static class PythonLife
{
    private const string PythonFolder = "python-3.12.7-embed-amd64"; // TODO: �K�؂ɕς���
    private const string PythonDll = "python312.dll"; // TODO: �K�؂ɕς���
    private const string PythonZip = "python312.zip"; // TODO: �K�؂ɕς���
    private const string PythonProject = "Scripts"; // TODO: �K�؂ɕς���

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PythonInitialize()
    {
        Application.quitting += PythonShutdown;
        Initialize(PythonProject);
    }
    private static void PythonShutdown()
    {
        Application.quitting -= PythonShutdown;
        Shutdown();
    }

    public static void Initialize(string appendPythonPath = "")
    {
        var pythonHome = $"{Application.streamingAssetsPath}/{PythonFolder}";
        var appendPath = string.IsNullOrWhiteSpace(appendPythonPath) ? string.Empty : $"{Application.streamingAssetsPath}/{appendPythonPath}";
        var pythonPath = string.Join(
            ";",
            $"{appendPath}",
            $"{pythonHome}/Lib/site-packages",
            $"{pythonHome}/{PythonZip}",
            $"{pythonHome}"
        );
        var scripts = $"{pythonHome}/Scripts";

        var path = Environment.GetEnvironmentVariable("PATH")?.TrimEnd(';');
        path = string.IsNullOrEmpty(path) ? $"{pythonHome};{scripts}" : $"{pythonHome};{scripts};{path};";
        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", $"{pythonHome}/Lib", EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", $"{pythonHome}/{PythonDll}", EnvironmentVariableTarget.Process);
#if UNITY_EDITOR
        Environment.SetEnvironmentVariable("PYTHONDONTWRITEBYTECODE", "1", EnvironmentVariableTarget.Process);
#endif

        PythonEngine.PythonHome = pythonHome;
        PythonEngine.PythonPath = pythonPath;

        PythonEngine.Initialize();
    }
    public static void Shutdown()
    {
        PythonEngine.Shutdown();
    }
}
