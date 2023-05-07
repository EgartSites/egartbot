using egartbot.CoreModules;
using System.Runtime.Loader;
using TdLib;
using TdLib.Bindings;

namespace egartbot
{
    public class Program
    {
        //api_id and api_hash from https://my.telegram.org/
        internal const int ApiId = 000000;
        internal const string ApiHash = "";

        internal static readonly TdClient _client = new();

        internal static readonly UpdatesRouter updatesRouter = new();

        internal static readonly ManualResetEventSlim ReadyToAuthenticate = new();
        internal static readonly ManualResetEventSlim KeepAlive = new();

        internal static Dictionary<string, LoadContext> loadedModules = new();

        //Initialize core modules below
        private static readonly AuthEngine authEngine = new();
        private static readonly ExternalModuleLoader externalModuleLoaderEngine = new();


        static void Main(string[] args)
        {
            _client.Bindings.SetLogVerbosityLevel(TdLogLevel.Fatal);

            _client.UpdateReceived += (_, update) =>
            {
                updatesRouter.Route(update);
            };

            ReadyToAuthenticate.Wait(); 

            KeepAlive.Wait();
        }

    }

    internal struct LoadContext
    {
        public AssemblyLoadContext AssemblyLoadContext;

        public HashSet<string> SubscribedToUpdates;
        public LoadContext(AssemblyLoadContext assemblyLoadContext)
        {
            SubscribedToUpdates = new HashSet<string>();
            AssemblyLoadContext = assemblyLoadContext;
        }
    }
}