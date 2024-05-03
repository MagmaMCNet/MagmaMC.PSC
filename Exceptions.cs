using System;
using System.Runtime.Serialization;

namespace MagmaMC.PSC
{
    public class AlreadyInitializedException : Exception, ISerializable
    {
        private readonly string method;
        public AlreadyInitializedException()
        {
        }
        public AlreadyInitializedException(string message)
        {
            method = message;
        }
        public override string ToString() => method;
    }
    public class NotInitializedException : Exception, ISerializable
    {
        private readonly string method;
        public NotInitializedException()
        {
        }
        public NotInitializedException(string message)
        {
            method = message;
        }
        public override string ToString() => method;
    }
    public class PlayerNotFoundException : Exception, ISerializable
    {
        private readonly string method;
        public PlayerNotFoundException()
        {
        }
        public PlayerNotFoundException(string message)
        {
            method = message;
        }
        public override string ToString() => method;
    }
    public class GroupNotFoundException : Exception, ISerializable
    {
        private readonly string method;
        public GroupNotFoundException()
        {
        }
        public GroupNotFoundException(string message)
        {
            method = message;
        }
        public override string ToString() => method;
    }
    public class GroupAlreadyExistsException : Exception, ISerializable
    {
        private readonly string method;
        public GroupAlreadyExistsException()
        {
        }
        public GroupAlreadyExistsException(string message)
        {
            method = message;
        }
        public override string ToString() => method;
    }
}
