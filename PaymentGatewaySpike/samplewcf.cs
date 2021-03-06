﻿using System.Configuration;
using PaymentGatewaySpike.ServiceReference1;


namespace PaymentGatewaySpike
{
    using System;
    using System.ServiceModel;

    class samplewcf
    {
        // Before using this example, replace the generic values with your merchant ID and password.
        private static readonly string _merchantId = ConfigurationManager.AppSettings["merchantid"];
        private static readonly string _transactionKey = ConfigurationManager.AppSettings["transactionkey"];

        static void Main(string[] args)
        {
            
            var request = new RequestMessage();

            request.merchantID = _merchantId;

            // Before using this example, replace the generic value with your
            // reference number for the current transaction.
            var referenceNumber = Guid.NewGuid();
            request.merchantReferenceCode = referenceNumber.ToString("N");

            // To help us troubleshoot any problems that you may encounter,
            // please include the following information about your application.
            request.clientLibrary = ".NET WCF";
            request.clientLibraryVersion = Environment.Version.ToString();
            request.clientEnvironment =
                Environment.OSVersion.Platform +
                Environment.OSVersion.Version.ToString();

            // This section contains a sample transaction request for the authorization 
            // service with complete billing, payment card, and purchase (two items) information.
            request.ccAuthService = new CCAuthService();
            request.ccAuthService.run = "true";

            BillTo billTo = new BillTo();
            billTo.firstName = "John";
            billTo.lastName = "Doe";
            billTo.street1 = "1295 Charleston Road";
            billTo.city = "Mountain View";
            billTo.state = "CA";
            billTo.postalCode = "94043";
            billTo.country = "US";
            billTo.email = "null@cybersource.com";
            billTo.ipAddress = "10.7.111.111";
            request.billTo = billTo;

            Card card = new Card();
            card.accountNumber = "4111111111111111";
            card.expirationMonth = "12";
            card.expirationYear = "2020";
            request.card = card;

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.currency = "USD";
            request.purchaseTotals = purchaseTotals;

            request.item = new Item[2];

            Item item = new Item();
            item.id = "0";
            item.unitPrice = "12.34";
            request.item[0] = item;

            item = new Item();
            item.id = "1";
            item.unitPrice = "56.78";
            request.item[1] = item;

            try
            {
                TransactionProcessorClient proc = new TransactionProcessorClient();

                proc.ChannelFactory.Credentials.UserName.UserName = request.merchantID;
                proc.ChannelFactory.Credentials.UserName.Password = _transactionKey;

                ReplyMessage reply = proc.runTransaction(request);

                // To retrieve individual reply fields, follow these examples.
                Console.WriteLine("decision = " + reply.decision);
                Console.WriteLine("reasonCode = " + reply.reasonCode);
                Console.WriteLine("requestID = " + reply.requestID);
                Console.WriteLine("requestToken = " + reply.requestToken);
                Console.WriteLine("ccAuthReply.reasonCode = " + reply.ccAuthReply.reasonCode);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("TimeoutException: " + e.Message + "\n" + e.StackTrace);
            }
            catch (FaultException e)
            {
                Console.WriteLine("FaultException: " + e.Message + "\n" + e.StackTrace);
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("CommunicationException: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                Console.ReadLine();
            }
        }

    }
}