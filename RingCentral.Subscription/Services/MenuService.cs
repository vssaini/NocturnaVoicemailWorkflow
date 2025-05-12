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
            Console.WriteLine("1. Create Subscription");
            Console.WriteLine("2. Fetch All Subscriptions");
            Console.WriteLine("3. Delete All Subscriptions");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice (1-4): ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await manager.CreateSubscriptionForNotificationAsync();
                    break;
                case "2":
                    await manager.FetchAllSubscriptionsAsync();
                    break;
                case "3":
                    await manager.DeleteAllSubscriptionsAsync();
                    break;
                case "4":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 4.");
                    break;
            }
        }
    }
}
