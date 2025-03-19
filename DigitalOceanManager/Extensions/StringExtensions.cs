namespace DigitalOceanManager.Extensions;

public static class StringExtensions
{
    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
    
    public static string ExtractProgramNameFromSupervisorConfig(this string supervisorConfig)
    {
        if (string.IsNullOrWhiteSpace(supervisorConfig))
            return "";

        var match = System.Text.RegularExpressions.Regex.Match(supervisorConfig, @"\[program:(\w+)]");
        return match.Success ? match.Groups[1].Value : "";
    }
    
    public static string EscapeForShell(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Replace("$", "\\$");
    }
}