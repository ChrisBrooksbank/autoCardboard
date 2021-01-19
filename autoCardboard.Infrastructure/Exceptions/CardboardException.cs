using System;

namespace autoCardboard.Infrastructure.Exceptions
{
    public class CardboardException: ApplicationException
    {
        public CardboardException(string message): base(message)
        {
        }
    }
}
