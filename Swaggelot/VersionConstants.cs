using System.Text.RegularExpressions;

namespace Swaggelot
{
    public static class VersionConstants
    {
        public const string VersionVariableName = "version";
        public const string VersionAnchor = "{version}";
        public static Regex VersionRegexp => new Regex(@"/api\/v(\d+)\/");
    }
}