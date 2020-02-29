using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Exceptions;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;
using TeleSharp.TL.Messages;

namespace TelegramCoinMiner.Commands
{
    class LaunchClickBotCommand : IAsyncCommand
    {
        public LaunchClickBotParams Params { get; set; }

        private ClickBotInfo _clickBotInfo;
        private TLUser _currentChannel;
        private TLMessage _adMessage;
        private int _adMessageNotFoundCount = 0;
        private int _followTheSameLink;
        private string _lastLink;
        private DateTime _startTime;

        public LaunchClickBotCommand(LaunchClickBotParams commandParams)
        {
            Params = commandParams;
            _clickBotInfo = ClickBotInfo.CreateDogecoinClickBotInfo();
            _startTime = DateTime.Now;
            _currentChannel = GetCurrentChannel().Result;
        }

        public async Task Execute()
        {
            try
            {
                while (!Params.TokenSource.Token.IsCancellationRequested)
                {
                    await CheckRunningTime();

                    _adMessage = await GetAdMessage();
                    _adMessageNotFoundCount = 0;
                    string link = GetAdLink();
                    await CheckDuplicatedAdLink(link);
                    await ExecuteWatchAdAndWaitForEndOfAdCommand();
                }
            }
            catch (AdMessageNotFoundException)
            {
                _adMessageNotFoundCount++;
                if (_adMessageNotFoundCount > 10)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + "Заданий нет, ждем 10 минут");
                    await Task.Delay(TimeSpan.FromMinutes(10));
                    _adMessageNotFoundCount = 0;
                }
                await ExecuteVisitCommand();
                await Task.Delay((_adMessageNotFoundCount + 1) * 1000);
            }
            catch (BrowserTimeoutException)
            {
                await ExecuteSkipAdCommand();
                await Task.Delay(1000);
            }
            catch (CapchaException)
            {
                await ExecuteSkipAdCommand();
                await Task.Delay(1500);
            }
            catch (ClickBotNotStartedException)
            {
                await ExecuteStartCommand();
                await Task.Delay(1000);
            }
            catch(TLSharp.Core.Network.Exceptions.FloodException ex) 
            {
                Console.WriteLine(ex.Message);
                await Task.Delay(ex.TimeToWait.Add(TimeSpan.FromMinutes(3)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                Console.WriteLine("Непредвиденная ошибка: " + ex.Message);
            }
        }

        private async Task CheckRunningTime()
        {
            if (DateTime.Now - _startTime > TimeSpan.FromMinutes(20))
            {
                Console.WriteLine("Прошло 20 минут, отдых");
                await Task.Delay(TimeSpan.FromMinutes(3));
                _startTime = DateTime.Now;
                await ExecuteVisitCommand();
                await Task.Delay(1000);
            }
        }

        private async Task CheckDuplicatedAdLink(string link)
        {
            if (_lastLink == link)
            {
                _followTheSameLink++;
                if (_followTheSameLink > 2)
                {
                    await ExecuteSkipAdCommand();
                }
            }
            else
            {
                _followTheSameLink = 0;
                _lastLink = link;
            }
        }

        private string GetAdLink()
        {
            return _adMessage.GetButtonWithUrl("go to website").Url;
        }

        private async Task ExecuteStartCommand()
        {
            var startCommand = new SendStartCommand(
                            new SendStartParams
                            {
                                Channel = _currentChannel,
                                TelegramClient = Params.TelegramClient,
                            });
            await startCommand.Execute();
        }

        private async Task ExecuteVisitCommand()
        {
            var visitCommand = new SendVisitCommand(
                            new SendVisitParams
                            {
                                Channel = _currentChannel,
                                TelegramClient = Params.TelegramClient
                            });
            await visitCommand.Execute();
        }

        private async Task ExecuteSkipAdCommand()
        {
            var skipAdCommand = new SkipAdCommand(
                            new SkipAdParams
                            {
                                Channel = _currentChannel,
                                TelegramClient = Params.TelegramClient,
                                AdMessage = _adMessage
                            });
            await skipAdCommand.Execute();
        }

        private async Task ExecuteWatchAdAndWaitForEndOfAdCommand()
        {
            var watchAndWaitForTheEndOfAdCommand = new MacroCommand(
                new List<IAsyncCommand>()
                {
                    new WatchAdCommand(
                        new WatchAdParams
                        {
                            Channel = _currentChannel,
                            TelegramClient = Params.TelegramClient,
                            Browser = Params.Browser,
                            AdMessage = _adMessage
                        }),
                    new WaitForTheEndOfAdCommand(
                        new WaitForTheEndOfAdParams
                        {
                            Channel = _currentChannel,
                            TelegramClient = Params.TelegramClient
                        })
                });
            await watchAndWaitForTheEndOfAdCommand.Execute();
        }

        private async Task<TLUser> GetCurrentChannel()
        {
            var foundChannels = await Params.TelegramClient.SearchUserAsync(_clickBotInfo.BotName);
            var currentChannel = foundChannels.Users.OfType<TLUser>()
                .FirstOrDefault(x => x.Username == _clickBotInfo.BotName && x.FirstName == _clickBotInfo.Title);
            return currentChannel;
        }

        private async Task<TLMessage> GetAdMessage()
        {
            var messages = await Params.TelegramClient.GetMessages(_currentChannel, Constants.ReadMessagesCount);

            if (messages == null)
            {
                throw new ClickBotNotStartedException();
            }

            var adMessage = messages.OfType<TLMessage>()
                .FirstOrDefault(x => x.Message.Contains("Press the \"Visit website\" button to earn"));

            if (adMessage == null)
            {
                throw new AdMessageNotFoundException();
            }

            return adMessage;
        }

        private async Task<string> ListenControll()
        {

            var dialogs = (TLDialogs) await Params.TelegramClient.GetUserDialogsAsync(); //получаем все диалоги

            var ControlPanel = dialogs.Chats
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.Title.ToLower() == "ControlPanel".ToLower());

            var command = await Params.TelegramClient.SendRequestAsync<TLChannelMessages>(new TLRequestGetHistory()
            {
                Peer = new TLInputPeerChannel { AccessHash = ControlPanel.AccessHash.Value, ChannelId = ControlPanel.Id },
                Limit = 1,
                AddOffset = 0,
                OffsetId = 0
            });

            var commandMessage = command.Messages.OfType<TLMessage>().FirstOrDefault(x => !x.Message.Contains("<Принято>"));

            string commandText = "noCommands";

            if (commandMessage != null) { commandText = commandMessage.Message; }


            await Params.TelegramClient.SendMessageAsync(new TLInputPeerChannel { ChannelId = ControlPanel.Id, AccessHash = ControlPanel.AccessHash.Value }, "<Принято>:" + commandText);
            

            return commandText;
        }
    }
}
