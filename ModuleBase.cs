using System.Runtime.CompilerServices;
using TdLib;

namespace egartbot.Modules
{
    public abstract class ModuleBase : Program
    {
        public static void Subscribe<T>(UpdatesRouter.UpdateHandler<T> toSubscribe, object source, [CallerMemberName] string caller = "")
        {
            updatesRouter.Subscribe(toSubscribe, source, caller);
        }
        
        public static async Task<T> ExecuteAsync<T>(TdApi.Function<T> function) where T : TdApi.Object
        {
            return await _client.ExecuteAsync(function);
        }
    }
}
