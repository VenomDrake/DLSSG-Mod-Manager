using System.Text;
using System.Text.RegularExpressions;

namespace DlssgManager.Core;

/// <summary>Bounded, read-only diagnostics for the final OptiScaler log session. Loading never proves rendered or generated frames.</summary>
public static class OptiScalerLogReader
{
    private const long MaximumLogBytes = 8L * 1024 * 1024;
    private const int MaximumLineBytes = 128 * 1024;
    private static readonly Regex Line = new(@"^(?:\[\d{2}:\d{2}:\d{2}\.\d{3,6}\]\s+\[([TDIWEFC])\]\s+|\d{2}:\d{2}:\d{2}\.\d{3,6}\t([TDIWEFC])\t)(.*)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    private static readonly Regex Header = new(@"^OptiScaler v\d+\.\d+[^\r\n]* loaded$", RegexOptions.CultureInvariant);

    public static LogVerification ReadLatest(string renderingDirectory, string? exeName = null)
    {
        try
        {
            var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(renderingDirectory));
            if (exeName is not null && Path.IsPathRooted(exeName) && !string.Equals(Path.GetDirectoryName(Path.GetFullPath(exeName)), root, StringComparison.OrdinalIgnoreCase)) return Missing();
            var path = Path.Combine(root, "OptiScaler.log");
            if (!File.Exists(path) || DetectionPaths.HasReparsePoint(path, root)) return Missing();
            return Read(path);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException) { return Missing(); }
    }

    public static LogVerification Read(string logPath)
    {
        bool loaded = false, partial = false;
        var errors = new List<string>();
        try
        {
            if ((File.GetAttributes(logPath) & FileAttributes.ReparsePoint) != 0) return Missing();
            using var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            long remaining = stream.Length;
            bool skipFirst = false;
            if (remaining > MaximumLogBytes)
            {
                long start = remaining - MaximumLogBytes;
                stream.Position = start - 1;
                skipFirst = stream.ReadByte() != '\n';
                stream.Position = start;
                remaining -= start;
                partial = true;
            }
            var buffer = new byte[8192];
            var bytes = new byte[MaximumLineBytes];
            int used = 0;
            bool oversized = false;
            while (remaining > 0)
            {
                int read = stream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                if (read == 0) break;
                remaining -= read;
                for (int i = 0; i < read; i++)
                {
                    if (buffer[i] != '\n')
                    {
                        if (!skipFirst && !oversized)
                        {
                            if (used < bytes.Length) bytes[used++] = buffer[i];
                            else { oversized = true; used = 0; loaded = false; partial = true; errors.Clear(); }
                        }
                        continue;
                    }
                    if (!skipFirst && !oversized)
                    {
                        var text = Encoding.UTF8.GetString(bytes, 0, used).TrimEnd('\r').TrimStart('\uFEFF');
                        var match = Line.Match(text);
                        if (match.Success)
                        {
                            string message = match.Groups[3].Value;
                            if (Header.IsMatch(message)) { loaded = true; partial = false; errors.Clear(); }
                            else if (loaded && (match.Groups[1].Value + match.Groups[2].Value).ToUpperInvariant() is "E" or "C" or "F")
                            {
                                if (errors.Count == 100) errors.RemoveAt(0);
                                errors.Add(message);
                            }
                        }
                    }
                    skipFirst = oversized = false;
                    used = 0;
                }
            }
            // An unfinished line may contain a new header or an error. Do not treat it as a complete session.
            if (used != 0 || oversized || skipFirst) partial = true;
            return new(loaded, false, false, null, null, null, errors,
                partial ? "opti_log_partial" : errors.Count != 0 ? "opti_log_errors" : loaded ? "opti_log_loaded" : "opti_log_missing")
            { PartialLog = partial, LogPath = Path.GetFullPath(logPath), LogTimestamp = File.GetLastWriteTimeUtc(logPath) };
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException) { return Missing(); }
    }

    private static LogVerification Missing() => new(false, false, false, null, null, null, [], "opti_log_missing");
}
