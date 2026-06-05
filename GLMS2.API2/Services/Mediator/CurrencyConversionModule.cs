using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    // Represents the currency conversion component in the mediator structure
    // Sends notifications when currency calculations are performed
    public class CurrencyConversionModule
    {
        private readonly IMediator _mediator;

        public CurrencyConversionModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Notifies the mediator when currency conversion occurs
        // Allows other components to respond if needed
        public void ConvertCurrency()
        {
            _mediator.Notify(this, "CurrencyConverted");
        }
    }
}