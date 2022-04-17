using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


public static class RegexHelper
{
    public static Dictionary<string, string> GetMatchCollection(string input, string pattern)
    {
        return Regex.Match(input, pattern).Groups.Where(x => x.Success).ToDictionary(key => key.Name, value => value.Captures.Single().Value);
    }
}
