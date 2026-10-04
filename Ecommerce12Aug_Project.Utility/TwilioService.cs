using Ecommerce12Aug_Project.Utility;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Ecommerce12Aug_Project.Utility
{
    public class TwilioService
    {
        private readonly TwilioSettings _twilioSettings;

        public TwilioService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        public void SendOrderSms(string personalPhoneNumber, int orderId)
        {
            TwilioClient.Init(
                _twilioSettings.AccountSid,
                _twilioSettings.AuthToken
            );

            var message = MessageResource.Create(
                body: "sms_order_confirmation",
                from: new PhoneNumber(_twilioSettings.PhoneNumber),
                to: new PhoneNumber(personalPhoneNumber)
            );

            Console.WriteLine("SMS SID: " + message.Sid);
            Console.WriteLine("SMS STATUS: " + message.Status);
        }

        public void MakeOrderCall(string personalPhoneNumber, int orderId)
        {
            TwilioClient.Init(
                _twilioSettings.AccountSid,
                _twilioSettings.AuthToken
            );

            var call = CallResource.Create(
                to: new PhoneNumber(personalPhoneNumber),
                from: new PhoneNumber(_twilioSettings.PhoneNumber),
                url: new Uri(
                    "https://broken-prologue-brilliant.ngrok-free.dev/Coustomer/Cart/OrderCall?orderId="
                    + orderId
                )
            );

            Console.WriteLine("CALL SID: " + call.Sid);
            Console.WriteLine("CALL STATUS: " + call.Status);
        }
    }
}