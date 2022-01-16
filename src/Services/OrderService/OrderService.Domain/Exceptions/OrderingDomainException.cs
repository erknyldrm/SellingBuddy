using System;

namespace OrderService.Domain.Exceptions
{
    internal class OrderingDomainException : Exception
    {
        public OrderingDomainException()
        {

        }

        public OrderingDomainException(string message)
            :base(message)
        {

        }

        public OrderingDomainException(string message, Exception innerException)
            :base(message, innerException)  
        {

        }
    }
}
