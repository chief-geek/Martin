namespace twtTest.Data
{
    public class UnexpectedFileReadException : Exception
    {
        public UnexpectedFileReadException() { }

        public UnexpectedFileReadException(string message) : base(message) { }
        public UnexpectedFileReadException(string message, Exception innerException) : base(message, innerException) { }
    }
}
