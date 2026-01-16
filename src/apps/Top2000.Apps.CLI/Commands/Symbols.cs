namespace Top2000.Apps.CLI.Commands;

public static class UnicodeSymbols
{
    public static string Check => OperatingSystem.IsWindows() ? "[Y]" : "✔";
    public static string Wrong => OperatingSystem.IsWindows() ? "[N]" : "✗";
    public static string Equal => "=";
    public static string Up => OperatingSystem.IsWindows() ? "^" : "↑";
    public static string Down => OperatingSystem.IsWindows() ? "v" : "↓";
    public static string New => OperatingSystem.IsWindows() ? "* " : "⚑";
    public static string Recurring => OperatingSystem.IsWindows() ? "~" : "↻";
    public static string Dash => OperatingSystem.IsWindows() ? "-" : "–";
    public static string Delta => OperatingSystem.IsWindows() ? "D" : "Δ";
}