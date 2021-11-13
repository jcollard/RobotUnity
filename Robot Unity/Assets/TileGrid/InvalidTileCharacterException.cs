using System;
using System.Runtime.Serialization;

[Serializable]
internal class InvalidTileCharacterException : Exception
{
    public InvalidTileCharacterException()
    {
    }

    public InvalidTileCharacterException(string message) : base(message)
    {
    }

    public InvalidTileCharacterException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidTileCharacterException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}