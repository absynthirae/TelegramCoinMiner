using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramCoinMiner.Commands.Params;
using TelegramCoinMiner.Exceptions;
using TelegramCoinMiner.Extensions;
using TeleSharp.TL;

namespace TelegramCoinMiner.Commands
{
    class LaunchClickBotCommand : IAsyncCommand
    {
        public LaunchClickBotParams Params { get; set; }
        private ClickBotSwitcher _botSwitcher;
        private TLUser _currentChannel = new TLUser();
        private TLMessage _adMessage;
        private int _adMessageNotFoundCount = 0;
        private DateTime startTime { get; set; }
        public LaunchClickBotCommand(LaunchClickBotParams commandParams)
        {
            Params = commandParams;
            _botSwitcher = new ClickBotSwitcher();
        }

        public async Task Execute()
        {
            startTime = DateTime.Now;
            try
            {
                if (_currentChannel.FirstName != _botSwitcher.CurrentBotInfo.Title &&
                    _currentChannel.Username != _botSwitcher.CurrentBotInfo.BotName)
                {
                    _currentChannel = await GetCurrentChannel();
                    await ExecuteSendVisitCommand(_currentChannel);
                    if (DateTime.Now - startTime > TimeSpan.FromMinutes(20)) { await Task.Delay(300000); }
                }


                while (!Params.TokenSource.Token.IsCancellationRequested)
                {
                    _adMessage = await GetAdMessage(_currentChannel);
                    await ExecuteWatchAdAndWaitForEndOfAdCommand(_currentChannel, _adMessage);
                   
                }
            }
            catch (AdMessageNotFoundException)
            {
                if (_adMessageNotFoundCount > 10)
                {
                    _botSwitcher.Next();
                    _adMessageNotFoundCount = 0;
                }
                // await ExecuteSendVisitCommand(_currentChannel);
            }
            catch (BrowserTimeoutException)
            {
                await ExecuteSkipCommand();
            }
            catch (CapchaException)
            {
                await ExecuteSkipCommand();
            }
            catch (ClickBotNotStartedException)
            {
                await ExecuteStartCommand(_currentChannel);
            }
            catch(TLSharp.Core.Network.Exceptions.FloodException ex) {
                Console.WriteLine("Too much of message");
                await Task.Delay(ex.TimeToWait);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Непредвиденная ошибка: " + ex.Message);
            }
            
        }

        private async Task ExecuteSendVisitCommand(TLUser currentChannel)
        {
            IAsyncCommand sendVisitCommand = new SendVisitCommand(new SendVisitParams
            {
                Channel = currentChannel,
                TelegramClient = Params.TelegramClient
            });
            await sendVisitCommand.Execute();
        }

        private async Task ExecuteStartCommand(TLUser currentChannel)
        {
            IAsyncCommand startCommand = new SendStartCommand(new SendStartParams
            {
                Channel = currentChannel,
                TelegramClient = Params.TelegramClient,
            });
            await startCommand.Execute();
        }

        private async Task ExecuteSkipCommand()
        {
            IAsyncCommand skipAdCommand = new SkipAdCommand(new SkipAdParams
            {
                Channel = _currentChannel,
                TelegramClient = Params.TelegramClient,
                AdMessage = _adMessage
            });
            await skipAdCommand.Execute();
        }

        private async Task<TLUser> GetCurrentChannel()
        {
            var currentBotInfo = _botSwitcher.CurrentBotInfo;
            var foundChannels = await Params.TelegramClient.SearchUserAsync(currentBotInfo.BotName);
            var currentChannel = foundChannels.Users.OfType<TLUser>().FirstOrDefault(x => x.Username == currentBotInfo.BotName && x.FirstName == currentBotInfo.Title);
            return currentChannel;
        }

        private async Task<TLMessage> GetAdMessage(TLUser currentChannel)
        {
            var messages = await Params.TelegramClient.GetMessages(currentChannel, Constants.ReadMessagesCount);

            if (messages == null)
            {
                throw new ClickBotNotStartedException();
            }

            var adMessage = messages.OfType<TLMessage>().FirstOrDefault(x => x.Message.Contains("Press the \"Visit website\" button to earn"));
            if (adMessage == null)
            {
                throw new AdMessageNotFoundException();
            }
            _adMessageNotFoundCount = 0;
            return adMessage;
        }

        private async Task ExecuteWatchAdAndWaitForEndOfAdCommand(TLUser currentChannel, TLMessage adMessage)
        {
            var commands = new List<IAsyncCommand>();
            commands.Add(new WatchAdCommand(
                        new WatchAdParams
                        {
                            Channel = currentChannel,
                            TelegramClient = Params.TelegramClient,
                            Browser = Params.Browser,
                            AdMessage = adMessage
                        }));
            commands.Add(new WaitForTheEndOfAdCommand(
                        new WaitForTheEndOfAdParams
                        {
                            Channel = currentChannel,
                            TelegramClient = Params.TelegramClient
                        }));
            IAsyncCommand macroCommand = new MacroCommand(commands);
            await macroCommand.Execute();
        }
    }
}
