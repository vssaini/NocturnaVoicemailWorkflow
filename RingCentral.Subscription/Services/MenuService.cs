namespace RingCentral.Subscription.Services;

public class MenuService(RcSubscriptionManager manager)
{
    /// <summary>
    /// Displays the interactive menu and handles user input.
    /// </summary>
    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select an action:");
            Console.WriteLine("1. Fetch All Extensions");
            Console.WriteLine("2. Create Subscription");
            Console.WriteLine("3. Fetch All Subscriptions");
            Console.WriteLine("4. Delete All Subscriptions");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice (1-5): ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await manager.FetchAllExtensionsAsync();
                    break;
                case "2":
                    await manager.CreateSubscriptionForNotificationAsync();
                    break;
                case "3":
                    await manager.FetchAllSubscriptionsAsync();
                    break;
                case "4":
                    await manager.DeleteAllSubscriptionsAsync();
                    break;
                case "5":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                    break;
            }
        }
    }

}
