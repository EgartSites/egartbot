# egartbot
## To deploy:
1. Go to https://my.telegram.org/ to get api_id and api_hash
2. Set AppId and AppHash in [Program.cs](/Program.cs)
```C#
  public class Program
  {
      //api_id and api_hash from https://my.telegram.org/
      internal const int ApiId = 000000;
      internal const string ApiHash = "";

      ...
  }
```
3. Build app

## How to use:
### In-chat commands (Just type in any telegram chat):

- Add new module
```
.madd 
```

- Remove module
```
.mremove MODULE_NAME
```

- Load module
```
.mload MODULE_NAME
```

- Unload module

```
.munload MODULE_NAME
```

- List installed modules
```
.mlist
```

## Module examples:
- [FakeScreenshot](https://github.com/EgartSites/egartbot-FakeScreenshot) - send fake screenshot notification (troll your friends)

- [FixKeyboardLayout](https://github.com/EgartSites/egartbot-FixKeyboardLayout) - fix keyboard layout (EN -> RU) like when you typed `Здравствуй`, but got `Plhfdcndeq`

## Tips for creating new modules:
1. Create new `.NET class library` project 
2. Install [TdSharp](https://www.nuget.org/packages/TDLib/) nuget package
3. Add builded `egartbot.dll` as project reference
4. Inherit `ModuleBase` from your class
5. Use same names for main class and project assembly file like:
`FakeScreenshot.cs` 
`FakeScreenshot.dll`
```C#
public class FakeScreenshot
```

6. Use `egartbot.Modules` namespace
7. Subscribe to updates from constructor (***Single subscribe to one update***)

> Subscribe<TdApi.Update.[UpdateType](https://core.telegram.org/tdlib/docs/classtd_1_1td__api_1_1_update.html)>(`METHOD`, this);

### Ex:
```C#
namespace egartbot.Modules
{
    public class FakeScreenshot : ModuleBase
    {
        readonly string commandPattern = "^[.]\\s*screenshot$";
        
        public FakeScreenshot()
        {
            Subscribe<TdApi.Update.UpdateNewMessage>(Process, this);
        }
        
        public async Task Process(TdApi.Update.UpdateNewMessage updateNewMessage)
        {
            ...
        }
        
        ...
    }
```


