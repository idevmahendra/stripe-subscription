using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stripe_subscription.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly string _defaultCustomerId;

        public SubscriptionController(IStripeService stripeService, IConfiguration config)
        {
            _stripeService = stripeService;
            _defaultCustomerId = config["StripeSettings:DefaultCustomerId"];
        }

        public async Task<IActionResult> ListPlans()
        {
            var plans = await _stripeService.ListSubscriptionPlansAsync();
            return View(plans);
        }

        public async Task<IActionResult> MyPlans()
        {
            var subscriptions = await _stripeService.ListSubscriptionPlansAsync(_defaultCustomerId);
            return View(subscriptions);
        }
        

        public async Task<IActionResult> Subscribe(string planId)
        {
            var plans = await _stripeService.SubscribeToPlanAsync(_defaultCustomerId, planId);
            return RedirectToAction("MyPlans");
        }

        public async Task<IActionResult> Unsubscribe(string subscriptionId)
        {
            var plans = await _stripeService.UnsubscribeFromPlanAsync(subscriptionId);
            return RedirectToAction("MyPlans");
        }
    }
}
