using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace CommandExecMcpServer
{
    [McpServerToolType]
    public static class CommandExecTools
    {
        [McpServerTool(Name = "exec")]
        [Description("Execute a system command via PowerShell (Windows) or Bash (Linux) and return stdout/stderr.")]
        public static async Task<string> ExecAsync(
            [Description("The command to execute (e.g. 'ls' or 'cmd.exe').")]
            string command,

            [Description("Arguments for the command (optional).")]
            string? args = null
        )
        {
            string combined = args is null ? command : $"{command} {args}";

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string shell;
            string arguments;

            if (isWindows)
            {
                // Escape for PowerShell
                combined = combined.Replace("\"", "`\"");

                string pwshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PowerShell", "7", "pwsh.exe");
                if (!File.Exists(pwshPath))
                    pwshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "PowerShell", "7", "pwsh.exe");

                shell = File.Exists(pwshPath) ? pwshPath : "powershell";
                arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{combined}\"";
            }
            else
            {
                // Escape for Bash
                combined = combined.Replace("\"", "\\\"");

                string bashPath = "/bin/bash";
                if (!File.Exists(bashPath))
                    bashPath = "/usr/bin/bash";

                shell = File.Exists(bashPath) ? bashPath : "bash";
                arguments = $"-c \"{combined}\"";
            }

            ProcessStartInfo psi = new()
            {
                FileName = shell,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = new() { StartInfo = psi };

            StringBuilder stdoutBuilder = new();
            StringBuilder stderrBuilder = new();

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    stdoutBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    stderrBuilder.AppendLine(e.Data);
            };

            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start process.");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            string stdout = stdoutBuilder.ToString().TrimEnd();
            string stderr = stderrBuilder.ToString().TrimEnd();

            if (string.IsNullOrEmpty(stderr))
            {
                return stdout;
            }

            if (string.IsNullOrEmpty(stdout))
            {
                return $"[stderr]\n{stderr}";
            }

            return $"[stdout]\n{stdout}\n\n[stderr]\n{stderr}";
        }
    }
}
