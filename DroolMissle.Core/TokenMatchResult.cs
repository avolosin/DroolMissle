namespace DroolMissle {
    public class TokenMatchResult
    {
        public string Token { get; set; }
        public string ExpectedJsonValue { get; set; }
        public string ActualJsonValue { get; set; }
        public bool IsMatch { get; set; }
        public bool IsCriteriaMatch { get; set; }
        public bool IsIgnored { get; set; }
    }
}