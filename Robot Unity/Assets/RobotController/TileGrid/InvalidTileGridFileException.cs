namespace RobotController
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class InvalidTileGridFileException : Exception
    {
        public InvalidTileGridFileException()
        {
        }

        public InvalidTileGridFileException(string message) : base(message)
        {
        }

        public InvalidTileGridFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTileGridFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}