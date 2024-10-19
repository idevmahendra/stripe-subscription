using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;


namespace stripe_subscription
{
    public interface IStripeService
    {
        Task<IEnumerable<Stripe.Plan>> ListSubscriptionPlansAsync();
        Task<IEnumerable<Stripe.Subscription>> ListSubscriptionPlansAsync(string customerId);
        Task<bool> SubscribeToPlanAsync(string customerId, string planId);
        Task<bool> UnsubscribeFromPlanAsync(string subscriptionId);
    }

    public class StripeService : IStripeService
    {
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["StripeSettings:SecretKey"];

            // Initialize Stripe API with the secret key
            StripeConfiguration.ApiKey = _secretKey;
        }

        // Method to list subscription plans
        public async Task<IEnumerable<Stripe.Plan>> ListSubscriptionPlansAsync()
        {
            var service = new PlanService();
            var options = new PlanListOptions
            {
                Active = true,
                Limit = 100,
            };

            return await service.ListAsync(options);
        }

        // Method to list subscription plans
        public async Task<IEnumerable<Stripe.Subscription>> ListSubscriptionPlansAsync(string customerId)
        {
            var subscriptionService = new SubscriptionService();

            // Create subscription list options with the customer filter
            var listOptions = new SubscriptionListOptions
            {
                Customer = customerId,
                Limit = 100 
            };

            // Fetch the subscriptions for the specified customer
            var subscriptions = await subscriptionService.ListAsync(listOptions);

            return subscriptions.Data;
        }

        // Method to subscribe a customer to a plan
        public async Task<bool> SubscribeToPlanAsync(string customerId, string planId)
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId,
                    },
                },
            };

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            return subscription.Status == "active";
        }

        // Method to unsubscribe from a plan
        public async Task<bool> UnsubscribeFromPlanAsync(string subscriptionId)
        {
            var service = new SubscriptionService();
            var subscription = await service.CancelAsync(subscriptionId);

            return subscription.Status == "canceled";
        }
    }
}
