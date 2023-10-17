using System.Diagnostics;
using System.IO;
using UnityEngine;
using DebugUnity = UnityEngine.Debug;

public class PythonScriptManagment : MonoBehaviour
{
    public int samples = 50;
    public bool transferXML = false; 
    public string remoteDir = "/home/ception/GIT/ception/UnityVSlam/data/UP_FOV=70_new/";
    public string recordingDir = "C:\\Users\\saarb\\UnityProjects\\ChessboardCalibration\\Recordings\\";
    private bool debug = false;

    public void StartPythonScript()
    {
        // Define the path to your Python executable (replace with your actual Python executable path)
        string pythonExecutable = "python";

        // Define the path to your Python script
        string pythonScript = "extraction.py";

        // Construct the desired working directory path based on the project's dataPath
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        string desiredWorkingDirectory = Path.Combine(projectPath, $"PythonTools\\{pythonScript}");

        string transfering_xml = transferXML ? "--transfer_xml" : "";

        // Construct the full command to run the Python script
        string command = $"{pythonExecutable} {desiredWorkingDirectory} {transfering_xml} --samples {samples} --remote_dir {remoteDir} --recording_dir {recordingDir}";

        // Create a process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Application.dataPath,
            };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        // Event handler for capturing output and errors
        process.OutputDataReceived += (sender, args) => DebugFilter(args.Data);
        process.ErrorDataReceived += (sender, args) => DebugFilter(args.Data);

        // Start the process
        process.Start();

        // Begin capturing output and errors
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Write the command to the standard input
        process.StandardInput.WriteLine(command);

        // Close the standard input to signal that no more input will be written
        process.StandardInput.Close();

        // Wait for the process to exit
        process.WaitForExit();

        // Check the exit code
        int exitCode = process.ExitCode;
        if (exitCode == 0)
        {
            DebugUnity.Log("Python script execution completed successfully.");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            DebugUnity.LogError($"Python script execution failed with exit code {exitCode}.");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void DebugFilter(string data)
    {
        if(data.Contains("python"))
            debug = true;
        else if(data == null)
            debug = false;
        else if(debug)
            DebugUnity.Log(data);
    }
}

