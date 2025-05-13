using RingCentral.Subscription.Helpers;
using RingCentral.Subscription.Models;

namespace RingCentral.Subscription.Services;

/// <summary>
/// Service class responsible for handling menu actions in the application.
/// </summary>
public class MenuService
{
    private readonly RcSubscriptionManager _manager;
    private readonly MenuActionHandler _actionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuService"/> class.
    /// </summary>
    /// <param name="manager">The <see cref="RcSubscriptionManager"/> to manage subscriptions.</param>
    public MenuService(RcSubscriptionManager manager)
    {
        _manager = manager;
        _actionHandler = new MenuActionHandler(manager);
    }

    /// <summary>
    /// Runs the menu system allowing the user to select actions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select an action:");
            Console.WriteLine($"{(int)MenuOption.FetchExtension}. Fetch Extension");
            Console.WriteLine($"{(int)MenuOption.FetchAllExtensions}. Fetch All Extensions");
            Console.WriteLine($"{(int)MenuOption.CreateSubscription}. Create Subscription");
            Console.WriteLine($"{(int)MenuOption.CreateSubscriptionForExtensions}. Create Subscription for Extensions");
            Console.WriteLine($"{(int)MenuOption.FetchSubscription}. Fetch Subscription");
            Console.WriteLine($"{(int)MenuOption.FetchAllSubscriptions}. Fetch All Subscriptions");
            Console.WriteLine($"{(int)MenuOption.AddExtensionsToSubscription}. Add Extensions to Subscription");
            Console.WriteLine($"{(int)MenuOption.RemoveExtensionsFromSubscription}. Remove Extensions from Subscription");
            Console.WriteLine($"{(int)MenuOption.DeleteSubscription}. Delete Subscription");
            Console.WriteLine($"{(int)MenuOption.DeleteAllSubscriptions}. Delete All Subscriptions");
            Console.WriteLine($"{(int)MenuOption.Exit}. Exit");
            Console.Write("Enter your choice (1-11): ");

            var input = Console.ReadLine();

            // Try to parse the user's input as a MenuOption
            if (Enum.TryParse<MenuOption>(input, out var option))
            {
                switch (option)
                {
                    case MenuOption.FetchExtension:
                        await _actionHandler.FetchExtensionAsync();
                        break;
                    case MenuOption.FetchAllExtensions:
                        await _manager.FetchAllExtensionsAsync();
                        break;
                    case MenuOption.CreateSubscription:
                        await _manager.CreateVoicemailSubscriptionAsync();
                        break;
                    case MenuOption.CreateSubscriptionForExtensions:
                        await _actionHandler.CreateSubscriptionForExtensionsAsync();
                        break;
                    case MenuOption.FetchSubscription:
                        await _actionHandler.FetchSubscriptionByIdAsync();
                        break;
                    case MenuOption.FetchAllSubscriptions:
                        await _manager.FetchAllSubscriptionsAsync();
                        break;
                    case MenuOption.AddExtensionsToSubscription:
                        await _actionHandler.AddExtensionsToSubscriptionAsync();
                        break;
                    case MenuOption.RemoveExtensionsFromSubscription:
                        await _actionHandler.RemoveExtensionsFromSubscriptionAsync();
                        break;
                    case MenuOption.DeleteSubscription:
                        await _actionHandler.DeleteSubscriptionByIdAsync();
                        break;
                    case MenuOption.DeleteAllSubscriptions:
                        await _manager.DeleteAllSubscriptionsAsync();
                        break;
                    case MenuOption.Exit:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 11.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid number between 1 and 9.");
            }
        }
    }
}
