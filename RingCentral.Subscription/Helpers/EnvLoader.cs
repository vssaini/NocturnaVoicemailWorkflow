using dotenv.net;
using RingCentral.Subscription.Models;

namespace RingCentral.Subscription.Helpers;

/// <summary>
/// Responsible for loading environment variables from the appropriate .env file
/// based on the provided application environment.
/// </summary>
public static class EnvLoader
{
    /// <summary>
    /// Gets the currently active environment.
    /// </summary>
    public static AppEnv CurrentEnvironment { get; private set; } = AppEnv.Local;

    /// <summary>
    /// Initializes the environment configuration by parsing the environment from arguments
    /// and loading the corresponding .env file.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void Initialize(string[] args)
    {
        CurrentEnvironment = ParseEnv(args);
        LoadEnvFile(CurrentEnvironment);

        ConsolePrinter.Info($"Loaded environment: {EnvLoader.CurrentEnvironment}");
    }

    /// <summary>
    /// Parses the environment from the command-line arguments.
    /// Defaults to <see cref="AppEnv.Local"/> if not provided or invalid.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>The parsed <see cref="AppEnv"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid environment is specified.</exception>
    private static AppEnv ParseEnv(string[] args)
    {
        if (args.Length == 0)
            return AppEnv.Local;

        return Enum.TryParse<AppEnv>(args[0], true, out var parsedEnv)
            ? parsedEnv
            : throw new ArgumentException("Invalid environment. Use: local, dev, or prod.");
    }

    /// <summary>
    /// Loads the environment variables from the corresponding `.env` file based on the specified environment.
    /// </summary>
    /// <param name="env">The target environment.</param>
    private static void LoadEnvFile(AppEnv env)
    {
        var envFile = env switch
        {
            AppEnv.Local => "local.env",
            AppEnv.Dev => "dev.env",
            AppEnv.Prod => "prod.env",
            _ => "local.env"
        };

        if (!File.Exists(envFile))
        {
            ConsolePrinter.Error($"Environment file '{envFile}' not found. Skipping load.");
            return;
        }

        // Ref - https://github.com/bolorundurowb/dotenv.net
        // Load environment variables from .env file
        var options = new DotEnvOptions(envFilePaths: [envFile]);
        DotEnv.Load(options);
    }
}