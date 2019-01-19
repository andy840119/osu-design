using System;

namespace osu.Framework.Design.Markup
{
    [Serializable]
    public class MarkupException : ApplicationException
    {
        public MarkupException() { }
        public MarkupException(string message) : base(message) { }
        public MarkupException(string message, System.Exception inner) : base(message, inner) { }
        protected MarkupException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}