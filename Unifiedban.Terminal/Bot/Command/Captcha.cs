﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Unifiedban.Models;
using Unifiedban.Models.Group;


namespace Unifiedban.Terminal.Bot.Command
{
    public class Captcha : ICommand
    {
        public void Execute(Message message) { }

        public void Execute(CallbackQuery callbackQuery)
        {
            long captchaId = Convert.ToInt64(callbackQuery.Data.Split(" ")[1]);

            if (captchaId != callbackQuery.From.Id)
                return;

            Manager.BotClient.DeleteMessageAsync(
                callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId);

            Manager.BotClient.RestrictChatMemberAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.From.Id,
                    new ChatPermissions()
                    {
                        CanSendMessages = true,
                        CanAddWebPagePreviews = true,
                        CanChangeInfo = true,
                        CanInviteUsers = true,
                        CanPinMessages = true,
                        CanSendMediaMessages = true,
                        CanSendOtherMessages = true,
                        CanSendPolls = true
                    });

            string name = callbackQuery.From.Username != null ? "@" + callbackQuery.From.Username : callbackQuery.From.FirstName;

            bool welcomeMessage = false;
            ConfigurationParameter welcomeMessageConfig = CacheData.GroupConfigs[callbackQuery.Message.Chat.Id]
                .Where(x => x.ConfigurationParameterId == "WelcomeMessage")
                .FirstOrDefault();
            if (welcomeMessageConfig != null)
                if (welcomeMessageConfig.Value.ToLower() == "true")
                    welcomeMessage = true;

            if (welcomeMessage)
            {
                MessageQueueManager.EnqueueMessage(
                   new ChatMessage()
                   {
                       Timestamp = DateTime.UtcNow,
                       Chat = callbackQuery.Message.Chat,
                       ParseMode = ParseMode.Html,
                       Text = Utils.Parsers.VariablesParser(
                           CacheData.Groups[callbackQuery.Message.Chat.Id].WelcomeText,
                           callbackQuery),
                   });

                return;
            }

            MessageQueueManager.EnqueueMessage(
                    new ChatMessage()
                    {
                        Timestamp = DateTime.UtcNow,
                        Chat = callbackQuery.Message.Chat,
                        ParseMode = ParseMode.Markdown,
                        Text = $"Thank you {name}!\nYou are now fully unlocked.",
                    });

        }
    }
}
