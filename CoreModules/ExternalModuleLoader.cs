using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using TdLib;

namespace egartbot.CoreModules
{
    internal class ExternalModuleLoader : Program
    {
        private readonly string commandPattern = "^[.]\\s*(?:(?:(mload|munload|mremove)\\s+(\\w+))|(madd|mlist))$";
        private readonly string assemblyPattern = "^\\w+[.]dll$";

        private long commandMessageId = 0;
        private long workInChatId = 0;

        private string assemblyName = "";
        bool waitForAssembly = false;


        public ExternalModuleLoader()
        {
            updatesRouter.Subscribe<TdApi.Update.UpdateNewMessage>(Process, this);
        }


        public async Task Process(TdApi.Update.UpdateNewMessage updateNewMessage)
        {
            if (updateNewMessage.Message.IsOutgoing)
            {
                string resultMessage;

                if (updateNewMessage.Message.Content is TdApi.MessageContent.MessageText messageText)
                {
                    var text = messageText.Text.Text;

                    Match match = Regex.Match(text, commandPattern, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        if (match.Groups[1].Value.ToLower() == "mload")
                        {
                            string moduleName = match.Groups[2].Value;

                            if (File.Exists("Modules/" + match.Groups[2].Value + ".dll"))
                            {
                                try
                                {
                                    var assemblyLoadContext = new AssemblyLoadContext(moduleName, true);

                                    Assembly assembly;

                                    using (FileStream fileStream = File.OpenRead("Modules/" + moduleName + ".dll"))
                                    {
                                        assembly = assemblyLoadContext.LoadFromStream(fileStream);
                                    }

                                    var type = assembly.GetType($"egartbot.Modules.{moduleName}");

                                    if (!loadedModules.ContainsKey(moduleName) && type != null)
                                    {
                                        loadedModules.Add(moduleName, new LoadContext(assemblyLoadContext));

                                        Activator.CreateInstance(type);

                                        resultMessage = $"{moduleName} module successfully loaded.";

                                        await _client.ExecuteAsync(new TdApi.EditMessageText
                                        {
                                            ChatId = updateNewMessage.Message.ChatId,
                                            MessageId = updateNewMessage.Message.Id,
                                            InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                            {
                                                Text = new TdApi.FormattedText
                                                {
                                                    Text = resultMessage,
                                                    Entities = new TdApi.TextEntity[]
                                                    {
                                                        new TdApi.TextEntity
                                                        {
                                                            Length = resultMessage.Length,
                                                            Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                    else
                                    {

                                        await _client.ExecuteAsync(new TdApi.EditMessageText
                                        {
                                            ChatId = updateNewMessage.Message.ChatId,
                                            MessageId = updateNewMessage.Message.Id,
                                            InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                            {
                                                Text = new TdApi.FormattedText
                                                {
                                                    Text = moduleName + " module already loaded.",
                                                    Entities = new TdApi.TextEntity[]
                                                    {
                                                        new TdApi.TextEntity
                                                        {
                                                            Length = (moduleName + " module already loaded.").Length,
                                                            Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                                catch (ArgumentException ae)
                                {
                                    Console.WriteLine(ae.Message);

                                    await _client.ExecuteAsync(new TdApi.EditMessageText
                                    {
                                        ChatId = updateNewMessage.Message.ChatId,
                                        MessageId = updateNewMessage.Message.Id,
                                        InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                        {
                                            Text = new TdApi.FormattedText
                                            {
                                                Text = moduleName + " module not loaded.",
                                                Entities = new TdApi.TextEntity[]
                                                {
                                                new TdApi.TextEntity
                                                    {
                                                        Length = (moduleName + " module not loaded.").Length,
                                                        Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);

                                    await _client.ExecuteAsync(new TdApi.EditMessageText
                                    {
                                        ChatId = updateNewMessage.Message.ChatId,
                                        MessageId = updateNewMessage.Message.Id,
                                        InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                        {
                                            Text = new TdApi.FormattedText
                                            {
                                                Text = moduleName + " module not loaded.",
                                                Entities = new TdApi.TextEntity[]
                                                {
                                                new TdApi.TextEntity
                                                {
                                                    Offset = 0,
                                                    Length = (moduleName + " module not loaded.").Length,
                                                    Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                }
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                            else
                            {
                                await _client.ExecuteAsync(new TdApi.EditMessageText
                                {
                                    ChatId = updateNewMessage.Message.ChatId,
                                    MessageId = updateNewMessage.Message.Id,
                                    InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                    {
                                        Text = new TdApi.FormattedText
                                        {
                                            Text = moduleName + " module doesn`t exist.",
                                            Entities = new TdApi.TextEntity[]
                                                {
                                                new TdApi.TextEntity
                                                {
                                                    Offset = 0,
                                                    Length = (moduleName + " module doesn`t exist.").Length,
                                                    Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                }
                                                }
                                        }
                                    }
                                });
                            }


                        }
                        else if (match.Groups[1].Value.ToLower() == "munload")
                        {
                            string moduleName = match.Groups[2].Value;

                            if (loadedModules.TryGetValue(moduleName, out LoadContext context))
                            {
                                foreach (var subscription in context.SubscribedToUpdates)
                                {
                                    updatesRouter.Unsubscribe(subscription, moduleName);
                                }
                            }

                            var weakRef = new WeakReference(loadedModules[moduleName].AssemblyLoadContext, trackResurrection: true);

                            loadedModules[moduleName].AssemblyLoadContext.Unload();

                            loadedModules.Remove(moduleName);

                            for (int i = 0; weakRef.IsAlive && (i < 10); i++)
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }

                            string resultText = $"{match.Groups[2].Value} module unloaded.";

                            await _client.ExecuteAsync(new TdApi.EditMessageText
                            {
                                ChatId = updateNewMessage.Message.ChatId,
                                MessageId = updateNewMessage.Message.Id,
                                InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                {
                                    Text = new TdApi.FormattedText
                                    {
                                        Text = resultText,
                                        Entities = new TdApi.TextEntity[]
                                        {
                                        new TdApi.TextEntity
                                        {
                                            Offset = 0,
                                            Length = resultText.Length,
                                            Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                        }
                                        }
                                    }
                                }
                            });

                        }
                        else if (match.Groups[3].Value.ToLower() == "madd")
                        {
                            if (workInChatId != 0)
                            {
                                await _client.ExecuteAsync(new TdApi.DeleteMessages
                                {
                                    ChatId = workInChatId,
                                    MessageIds = new long[] { commandMessageId },
                                    Revoke = true
                                });
                            }

                            commandMessageId = updateNewMessage.Message.Id;
                            workInChatId = updateNewMessage.Message.ChatId;

                            if (updateNewMessage.Message.ReplyToMessageId == 0)
                            {
                                resultMessage = "Please, reply to this message with module assembly file. (*NO SPACES*.dll ex. FirstTest.dll)";

                                await _client.ExecuteAsync(new TdApi.EditMessageText
                                {
                                    ChatId = workInChatId,
                                    MessageId = commandMessageId,
                                    InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                    {
                                        Text = new TdApi.FormattedText
                                        {
                                            Text = resultMessage,
                                            Entities = new TdApi.TextEntity[]
                                            {
                                                new TdApi.TextEntity
                                                {
                                                    Length = resultMessage.Length,
                                                    Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                }
                                            }
                                        }
                                    }
                                });

                                waitForAssembly = true;
                            }
                            else
                            {
                                var selectedFileMessage = await _client.ExecuteAsync(new TdApi.GetMessage
                                {
                                    ChatId = workInChatId,
                                    MessageId = updateNewMessage.Message.ReplyToMessageId
                                });

                                int assemblyFileId = ((TdApi.MessageContent.MessageDocument)selectedFileMessage.Content).Document.Document_.Id;
                                assemblyName = ((TdApi.MessageContent.MessageDocument)selectedFileMessage.Content).Document.FileName;

                                match = Regex.Match(assemblyName, assemblyPattern);

                                if (match.Success)
                                {
                                    if (!File.Exists("Modules/" + assemblyName))
                                    {
                                        await DownloadModuleAssembly(assemblyFileId);
                                    }
                                    else
                                    {
                                        resultMessage = "That module already exists.";

                                        await _client.ExecuteAsync(new TdApi.EditMessageText
                                        {
                                            ChatId = workInChatId,
                                            MessageId = commandMessageId,
                                            InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                            {
                                                Text = new TdApi.FormattedText
                                                {
                                                    Text = resultMessage,
                                                    Entities = new TdApi.TextEntity[]
                                                    {
                                                        new TdApi.TextEntity
                                                        {
                                                            Length = resultMessage.Length,
                                                            Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    resultMessage = "Wrong file.";

                                    await _client.ExecuteAsync(new TdApi.EditMessageText
                                    {
                                        ChatId = workInChatId,
                                        MessageId = commandMessageId,
                                        InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                        {
                                            Text = new TdApi.FormattedText
                                            {
                                                Text = resultMessage,
                                                Entities = new TdApi.TextEntity[]
                                                {
                                                    new TdApi.TextEntity
                                                    {
                                                        Length = resultMessage.Length,
                                                        Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }

                            }
                        }
                        else if (match.Groups[1].Value.ToLower() == "mremove")
                        {
                            if (File.Exists("Modules/" + match.Groups[2].Value + ".dll"))
                            {
                                File.Delete("Modules/" + match.Groups[2].Value + ".dll");

                                resultMessage = $"{match.Groups[2].Value} module successfully removed.";

                                await _client.ExecuteAsync(new TdApi.EditMessageText
                                {
                                    ChatId = updateNewMessage.Message.ChatId,
                                    MessageId = updateNewMessage.Message.Id,
                                    InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                    {
                                        Text = new TdApi.FormattedText
                                        {
                                            Text = resultMessage,
                                            Entities = new TdApi.TextEntity[]
                                            {
                                                new TdApi.TextEntity
                                                {
                                                    Length = resultMessage.Length,
                                                    Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                resultMessage = $"{match.Groups[2].Value} module not found.";

                                await _client.ExecuteAsync(new TdApi.EditMessageText
                                {
                                    ChatId = updateNewMessage.Message.ChatId,
                                    MessageId = updateNewMessage.Message.Id,
                                    InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                    {
                                        Text = new TdApi.FormattedText
                                        {
                                            Text = resultMessage,
                                            Entities = new TdApi.TextEntity[]
                                            {
                                                new TdApi.TextEntity
                                                {
                                                    Length = resultMessage.Length,
                                                    Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                        }
                        else if (match.Groups[3].Value.ToLower() == "mlist")
                        {
                            var modules = new List<string>();

                            foreach (var module in Directory.GetFiles("Modules"))
                            {
                                var moduleName = module.Replace("Modules\\", "")[0..^4];

                                if (loadedModules.ContainsKey(moduleName))
                                    moduleName = "✅ " + moduleName;

                                modules.Add(moduleName);
                            }

                            modules.Sort((string a, string b) => {
                                if (a.Contains("✅"))
                                    return -1;

                                if (b.Contains("✅"))
                                    return 1;

                                return 0;
                            });

                            resultMessage = "Available modules (✅ -  loaded):";

                            var textEntities = new List<TdApi.TextEntity>()
                            {
                                
                                new TdApi.TextEntity
                                {
                                    Length = resultMessage.Length,
                                    Type = new TdApi.TextEntityType.TextEntityTypeBold()
                                }
                                
                            };

                            foreach (var m in modules)
                            {
                                resultMessage += Environment.NewLine;

                                int offset = (m.Contains("✅") ? 2 : 0);
                                
                                textEntities.Add(new TdApi.TextEntity
                                {
                                    Offset = resultMessage.Length + offset,
                                    Length = m.Length,
                                    Type = new TdApi.TextEntityType.TextEntityTypeCode()
                                });

                                resultMessage += m;
                            }

                            await _client.ExecuteAsync(new TdApi.EditMessageText
                            {
                                ChatId = updateNewMessage.Message.ChatId,
                                MessageId = updateNewMessage.Message.Id,
                                InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                {
                                    Text = new TdApi.FormattedText
                                    {
                                        Text = resultMessage,
                                        Entities = textEntities.ToArray()
                                    }
                                }
                            });
                        }
                    }
                }
                else if (waitForAssembly && updateNewMessage.Message.Content is TdApi.MessageContent.MessageDocument messageDocument && updateNewMessage.Message.ReplyToMessageId == commandMessageId && updateNewMessage.Message.ChatId == workInChatId)
                {
                    int assemblyFileId = messageDocument.Document.Document_.Id;
                    assemblyName = messageDocument.Document.FileName;

                    Match match = Regex.Match(assemblyName, assemblyPattern);

                    if (match.Success)
                    {
                        if (!File.Exists("Modules/" + assemblyName))
                        {
                            await DownloadModuleAssembly(assemblyFileId, updateNewMessage.Message.Id);

                            waitForAssembly = false;
                        }
                        else
                        {
                            await _client.ExecuteAsync(new TdApi.DeleteMessages
                            {
                                ChatId = workInChatId,
                                MessageIds = new long[] { commandMessageId },
                                Revoke = true
                            });

                            resultMessage = "That module already exist.";

                            await _client.ExecuteAsync(new TdApi.SendMessage
                            {
                                ChatId = workInChatId,
                                ReplyToMessageId = updateNewMessage.Message.Id,
                                InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                                {
                                    Text = new TdApi.FormattedText
                                    {
                                        Text = resultMessage,
                                        Entities = new TdApi.TextEntity[]
                                        {
                                            new TdApi.TextEntity
                                            {
                                                Length = resultMessage.Length,
                                                Type = new TdApi.TextEntityType.TextEntityTypeBold()
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        await _client.ExecuteAsync(new TdApi.DeleteMessages
                        {
                            ChatId = workInChatId,
                            MessageIds = new long[] { commandMessageId },
                            Revoke = true
                        });

                        resultMessage = "Wrong file.";

                        await _client.ExecuteAsync(new TdApi.SendMessage
                        {
                            ChatId = workInChatId,
                            ReplyToMessageId = updateNewMessage.Message.Id,
                            InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                            {
                                Text = new TdApi.FormattedText
                                {
                                    Text = resultMessage,
                                    Entities = new TdApi.TextEntity[]
                                    {
                                        new TdApi.TextEntity
                                        {
                                            Length = resultMessage.Length,
                                            Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }
        }

        private async Task DownloadModuleAssembly(int assemblyFileId, long documentMessageId = 0)
        {
            if (!Directory.Exists("Modules"))
            {
                Directory.CreateDirectory("Modules");
            }

            TdApi.File downloadedFile = await _client.ExecuteAsync(new TdApi.DownloadFile
            {
                FileId = assemblyFileId,
                Limit = 0,
                Offset = 0,
                Priority = 32,
                Synchronous = true
            });

            File.Move(downloadedFile.Local.Path, "Modules/" + assemblyName);

            if (documentMessageId != 0)
            {
                await _client.ExecuteAsync(new TdApi.DeleteMessages
                {
                    ChatId = workInChatId,
                    MessageIds = new long[] { documentMessageId },
                    Revoke = true
                });
            }

            await _client.ExecuteAsync(new TdApi.EditMessageText
            {
                ChatId = workInChatId,
                MessageId = commandMessageId,
                InputMessageContent = new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = "Added " + assemblyName[0..^4] + " module.",
                        Entities = new TdApi.TextEntity[]
                        {
                            new TdApi.TextEntity
                            {
                                Length = ("Added " + assemblyName[0..^4] + " module.").Length,
                                Type = new TdApi.TextEntityType.TextEntityTypeBold { }
                            }
                        }
                    }
                }
            });

            workInChatId = 0;
        }
    }
}
