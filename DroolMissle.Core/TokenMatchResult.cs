using System.Diagnostics;
namespace DroolMissle {
    [DebuggerDisplay("IsMatch={IsMatch} Token={Token} Expected={ExpectedJsonValue} Actual={ActualJsonValue} MatchDescription={MatchDescription}")]
    public class TokenMatchResult
    {
        public string Token { get; set; }
        public string ExpectedJsonValue { get; set; }
        public string ActualJsonValue { get; set; }
        public bool IsMatch { get; set; }
        public bool IsTokenMatchApplied { get; set; }
        public bool IsIgnored { get; set; }
        public string MatchDescription { get; internal set; }
    }
}