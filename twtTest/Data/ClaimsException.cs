namespace twtTest.Data
{
    public class ClaimsException : Exception
    {
        public ClaimsException() { }

        public ClaimsException(string message) : base(message) { }
        public ClaimsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
