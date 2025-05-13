using RingCentral.Subscription.Services;

namespace RingCentral.Subscription.Helpers
{
    /// <summary>
    /// Handles various menu actions related to subscription management,
    /// including fetching extension details, creating subscriptions, 
    /// and deleting subscriptions based on user input.
    /// </summary>
    public class MenuActionHandler
    {
        private readonly RcSubscriptionManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuActionHandler"/> class.
        /// </summary>
        /// <param name="manager">The manager responsible for handling subscription actions.</param>
        public MenuActionHandler(RcSubscriptionManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Prompts the user to enter an extension ID and fetches the extension details.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task FetchExtensionAsync()
        {
            Console.Write("Enter extension number: ");
            var extensionId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(extensionId))
            {
                Console.WriteLine("No extension Id entered. Aborting.");
                return;
            }

            await _manager.FetchExtensionAsync(extensionId.Trim());
        }

        /// <summary>
        /// Prompts the user to enter a list of extension IDs and creates subscriptions for those extensions.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateSubscriptionForExtensionsAsync()
        {
            Console.Write("Enter extension IDs separated by commas: ");
            var extInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(extInput))
            {
                Console.WriteLine("No extension IDs entered. Aborting.");
                return;
            }

            try
            {
                var extensionIds = extInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id.Trim()))
                    .ToArray();

                await _manager.CreateVoicemailSubscriptionAsync(extensionIds);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter numeric extension IDs separated by commas.");
            }
        }

        /// <summary>
        /// Prompts the user to enter a subscription ID and fetches the subscription details.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task FetchSubscriptionByIdAsync()
        {
            Console.Write("Enter the subscription ID to fetch: ");
            var subscriptionId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                Console.WriteLine("No subscription ID entered. Aborting.");
                return;
            }

            await _manager.FetchSubscriptionAsync(subscriptionId.Trim());
        }

        /// <summary>
        /// Prompts the user to enter a subscription ID and a list of extension IDs to add to the subscription.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddExtensionsToSubscriptionAsync()
        {
            Console.Write("Enter the subscription ID: ");
            var subscriptionId = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                Console.WriteLine("No subscription ID entered. Aborting.");
                return;
            }

            Console.Write("Enter extension IDs to add (comma-separated): ");
            var extInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(extInput))
            {
                Console.WriteLine("No extension IDs entered. Aborting.");
                return;
            }

            try
            {
                var extensionIds = extInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id.Trim()))
                    .ToArray();

                await _manager.AddExtensionsToSubscriptionAsync(subscriptionId, extensionIds);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter numeric extension IDs separated by commas.");
            }
        }

        /// <summary>
        /// Prompts the user to enter a subscription ID and a list of extension IDs to remove from the subscription.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveExtensionsFromSubscriptionAsync()
        {
            Console.Write("Enter the subscription ID: ");
            var subscriptionId = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                Console.WriteLine("No subscription ID entered. Aborting.");
                return;
            }

            Console.Write("Enter extension IDs to remove (comma-separated): ");
            var extInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(extInput))
            {
                Console.WriteLine("No extension IDs entered. Aborting.");
                return;
            }

            try
            {
                var extensionIds = extInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id.Trim()))
                    .ToArray();

                await _manager.RemoveExtensionsFromSubscriptionAsync(subscriptionId, extensionIds);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter numeric extension IDs separated by commas.");
            }
        }

        /// <summary>
        /// Prompts the user to enter a subscription ID and deletes the corresponding subscription.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteSubscriptionByIdAsync()
        {
            Console.Write("Enter the subscription ID to delete: ");
            var subscriptionId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                Console.WriteLine("No subscription ID entered. Aborting.");
                return;
            }

            await _manager.DeleteSubscriptionAsync(subscriptionId.Trim());
        }
    }
}
