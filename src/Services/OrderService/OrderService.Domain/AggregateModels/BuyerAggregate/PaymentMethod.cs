using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;
using System;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class PaymentMethod : BaseEntity
    {
        public string Alias { get; set; }
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime Expiration { get; set; }
        public int CardTypeId { get; set; }
        public CardType CardType { get; private set; }

        public PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            CardNumber = !String.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            SecurityNumber = !String.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            CardHolderName = !String.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
                throw new OrderingDomainException(nameof(expiration));

            Alias = alias;
            Expiration = expiration;
            CardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return CardTypeId == cardTypeId && CardNumber == cardNumber && Expiration == expiration;
        }
    }
}
