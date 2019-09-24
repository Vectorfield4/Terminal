﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Unifiedban.Terminal.Bot.Command
{
    public class Start : ICommand
    {
        public Task Execute(Message message)
        {
            if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private
                || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Channel) {
                if(MessageQueueManager.AddChatIfNotPresent(message.Chat.Id))
                    return Manager.BotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Your chat {message.Chat.Title} has been added successfully!"
                    );

                return Manager.BotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Error adding chat {message.Chat.Title}! Please contact our support"
                    );
            }

            if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group
                || message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)
            {
                if (MessageQueueManager.AddGroupIfNotPresent(new Models.Group.TelegramGroup()
                {
                    TelegramChatId = message.Chat.Id
                }))
                    return Manager.BotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Your group {message.Chat.Title} has been added successfully!"
                    );

                return Manager.BotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Error adding group {message.Chat.Title}! Please contact our support"
                    );
            }

            return Manager.BotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Error: chat type not recognized. Please contact our support."
            );
        }
    }
}
