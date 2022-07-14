using System.Net;

namespace DroolMissle.JsonCompare
{
    public class TokenMatchCriteria
    {
        private Func<string, string, bool> _matcher;
        public string Description { get; set; }


        /// <summary>
        /// The string starts with a parseable date - will split a string on spaces and check that index zero can be parsed via DateTime.Parse
        /// </summary>
        public static TokenMatchCriteria StartsWithDateTime(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => DateTime.TryParse(d?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)[0], out var _), $"'{propName}' should be a valid date");
        }
        public static TokenMatchCriteria AnyDate(string propName, bool allowNulls = false)
        {
            if (allowNulls)
            {
                return new TokenMatchCriteria(propName, (expected, d) => d == "null" || DateTime.TryParse(d, out var _), $"'{propName}' should be a valid date");
            }
            return new TokenMatchCriteria(propName, (expected, d) => DateTime.TryParse(d, out var _), $"'{propName}' should be a valid date");
        }

        public static TokenMatchCriteria AnyIpV4Address(string propName)
        {    
            return new TokenMatchCriteria(propName, (expected, d) => IPAddress.TryParse(d, out var ip) && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork, $"'{propName}' should be a valid IP Address");
        }

        public static TokenMatchCriteria AnyFutureDateUtc(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => DateTime.TryParse(d, out var date) && date >= DateTime.UtcNow, $"'{propName}' should be a valid date in the future");
        }

        public static TokenMatchCriteria AnyGuid(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => Guid.TryParse(d, out var _), $"'{propName}' should be a valid guid");
        }

        public static TokenMatchCriteria AnyInteger(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => int.TryParse(d, out var _), $"'{propName}' should be a valid integer");
        }

        public static TokenMatchCriteria Ignore(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => true, $"Property was excluded but somehow didn't match!? WTF?!");
        }
        public static TokenMatchCriteria AnyBoolean(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, d) => bool.TryParse(d, out var _), $"'{propName}' should be a valid boolean");
        }

        public static TokenMatchCriteria DecimalBetween(string propName, decimal min, decimal max)
        {
            return new TokenMatchCriteria(propName, (expected, value) => decimal.TryParse(value, out var numberValue) && numberValue >= min && numberValue <= max, $"'{propName}' should be a decimal between {min} and {max}");
        }
        public static TokenMatchCriteria IntegerBetween(string propName, int min, int max)
        {
            return new TokenMatchCriteria(propName, (expected, value) => int.TryParse(value, out var numberValue) && numberValue >= min && numberValue <= max, $"'{propName}' should be a decimal between {min} and {max}");
        }

        public static TokenMatchCriteria NotNullOrWhiteSpace(string propName)
        {
            return new TokenMatchCriteria(propName, (expected, s) => !String.IsNullOrWhiteSpace(s), $"'{propName}' should not be null, empty or whitespace");
        }

        public static TokenMatchCriteria ShouldMatch(string propName, string exactValue)
        {
            return new TokenMatchCriteria(propName, (expected, s) => s == exactValue, $"'{propName}' should match exactly");
        }
        public static TokenMatchCriteria ShouldMatch(string propName, Guid exactValue)
        {
            return new TokenMatchCriteria(propName, (expected, s) => Guid.TryParse(s, out var g) && g == exactValue, $"'{propName}' should match exactly");
        }
        public static TokenMatchCriteria ShouldMatch(string propName, Guid? exactValue)
        {
            return new TokenMatchCriteria(propName, (expected, s) => Guid.TryParse(s, out var g) && g == exactValue, $"'{propName}' should match exactly");
        }

        public static TokenMatchCriteria ShouldMatch(string propName, int exactValue)
        {
            return new TokenMatchCriteria(propName, (expected, s) => int.TryParse(s, out var i) && i == exactValue, $"'{propName}' should match exactly");
        }

        public static TokenMatchCriteria IgnoreAllSubProperties(string propName)
        {
            return new TokenMatchCriteria(propName + ".*-", (expected, s) => true, "Ignored Property");
        }

        public TokenMatchCriteria(string propertyPath, Func<string, string, bool> matcher, string description)
        {
            PropertyPath = propertyPath;
            _matcher = matcher;
            Description = description;
        }
        public string PropertyPath { get; }
        internal bool Matches(string expected, string actual)
        {
            var matches = _matcher.Invoke(expected, actual);
            return matches;
        }

        
    }
}
