using TdLib;

namespace egartbot.CoreModules
{
    internal class AuthEngine : Program
    { 
        public AuthEngine()
        {
            updatesRouter.Subscribe<TdApi.Update.UpdateAuthorizationState>(ProcessAuthorizationState, this);
        }


        public async Task ProcessAuthorizationState(TdApi.Update.UpdateAuthorizationState UpdateAuthorizationState)
        {
            switch (UpdateAuthorizationState.AuthorizationState)
            {
                case TdApi.AuthorizationState.AuthorizationStateReady:
                    ReadyToAuthenticate.Set();
                    break;

                case TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters:
                    await _client.ExecuteAsync(new TdApi.SetTdlibParameters
                    {
                        ApiId = ApiId,
                        ApiHash = ApiHash,
                        DeviceModel = "PC",
                        SystemLanguageCode = "en",
                        ApplicationVersion = "1.0",
                        DatabaseDirectory = "Storage",
                        FilesDirectory = "Storage",
                    });
                    break;

                case TdApi.AuthorizationState.AuthorizationStateWaitCode:
                    Console.Write("Login code: ");
                    var code = Console.ReadLine();

                    await _client.ExecuteAsync(new TdApi.CheckAuthenticationCode
                    {
                        Code = code
                    });
                    break;

                case TdApi.AuthorizationState.AuthorizationStateWaitPassword:
                    Console.WriteLine("2FA turned on");
                    Console.Write("Password: ");
                    var password = Console.ReadLine();
                    
                    await _client.ExecuteAsync(new TdApi.CheckAuthenticationPassword
                    {
                        Password = password
                    });
                    break;

                case TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber:
                    Console.Write("Phone number: ");
                    var phoneNumber = Console.ReadLine();
                    
                    await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
                    {
                        PhoneNumber = phoneNumber
                    });
                    break;

            }
        }
    }
}
