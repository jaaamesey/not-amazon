using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Studio1BTask.Services
{
    public class PaypalService
    {
        private static string _clientId;
        private static string _secret;

        public PayPalHttpClient Client()
        {
            _clientId = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_PayPalClient");
            _secret = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_PayPalSecret");

            // Creating a sandbox environment
            var environment = new SandboxEnvironment(_clientId, _secret);

            // Creating a client for the environment
            var client = new PayPalHttpClient(environment);
            return client;
        }

        public async Task<Order> CreateOrder(List<Item> items, decimal totalPrice)
        {
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            var order = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",

                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "AUD",
                            Value = totalPrice.ToString(CultureInfo.InvariantCulture),
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "AUD",
                                    Value = totalPrice.ToString(CultureInfo.InvariantCulture)
                                }
                            }
                        },
                        Items = items
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "http://studio1btask.azurewebsites.net/",
                    CancelUrl = "http://studio1btask.azurewebsites.net/"
                }
            };
            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            var response = await Client().Execute(request);
            var result = response.Result<Order>();
            return result;
        }

        public async Task<Order> CaptureOrder(string approvedOrderId)
        {
            // Construct a request object and set desired parameters
            // Replace ORDER-ID with the approved order id from create order
            var request = new OrdersCaptureRequest(approvedOrderId);
            request.RequestBody(new OrderActionRequest());
            var response = await Client().Execute(request);
            var getRequest = new OrdersGetRequest(approvedOrderId);
            response = await Client().Execute(getRequest);
            var result = response.Result<Order>();
            return result;
        }
    }
}