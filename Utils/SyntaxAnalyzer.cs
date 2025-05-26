using System.Diagnostics;
using System.Text;

namespace BotJDM.Utils;

public static class SyntaxAnalyzer
{
    public static async Task<string> AnalyzeWithPython(string input)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/home/vixy/RiderProjects/JDMBOT-TER/.venv/bin/python3",
            Arguments = "analyze.py",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        var process = new Process { StartInfo = psi };
        process.Start();

        await process.StandardInput.WriteAsync(input);
        process.StandardInput.Close();

        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        return result;
    }
}