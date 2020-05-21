﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Unifiedban.Terminal.Bot.Command
{
    public class Leave : ICommand
    {
        public void Execute(Message message)
        {
            Manager.BotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            if (!Utils.BotTools.IsUserOperator(message.From.Id) &&
               !Utils.ChatTools.IsUserAdmin(message.Chat.Id, message.From.Id))
            {
                MessageQueueManager.EnqueueMessage(
                   new ChatMessage()
                   {
                       Timestamp = DateTime.UtcNow,
                       Chat = message.Chat,
                       ReplyToMessageId = message.MessageId,
                       Text = CacheData.GetTranslation("en", "error_not_auth_command")
                   });
                Manager.BotClient.SendTextMessageAsync(
                    chatId: CacheData.ControlChatId,
                    parseMode: ParseMode.Markdown,
                    text: String.Format(
                        "User *{0}:{1}* tried to use command Leave.",
                        message.From.Id,
                        message.From.Username)
                );
                return;
            }

            List<InlineKeyboardButton> confirmationButton = new List<InlineKeyboardButton>();
            confirmationButton.Add(InlineKeyboardButton.WithCallbackData(
                        CacheData.GetTranslation("en", "yes", true),
                        $"/Leave yes"
                        ));
            confirmationButton.Add(InlineKeyboardButton.WithCallbackData(
                        CacheData.GetTranslation("en", "no", true),
                        $"/Leave no"
                        ));

            Manager.BotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        parseMode: ParseMode.Markdown,
                        text: "*[ADMIN]*\nAre you sure you want to leave?",
                        replyMarkup: new InlineKeyboardMarkup(
                            confirmationButton
                        )
                    );

            //MessageQueueManager.EnqueueMessage(
            //        new ChatMessage()
            //        {
            //            Timestamp = DateTime.UtcNow,
            //            Chat = message.Chat,
            //            ParseMode = ParseMode.Markdown,
            //            Text = "*[ADMIN]*\nAre you sure you want to leave?",
            //            ReplyMarkup = new InlineKeyboardMarkup(
            //                confirmationButton
            //            )
            //        });
        }

        public void Execute(CallbackQuery callbackQuery)
        {
            if (!Utils.BotTools.IsUserOperator(callbackQuery.From.Id) &&
               !Utils.ChatTools.IsUserAdmin(callbackQuery.Message.Chat.Id,
               callbackQuery.From.Id))
            {
                Manager.BotClient.SendTextMessageAsync(
                    chatId: CacheData.ControlChatId,
                    parseMode: ParseMode.Markdown,
                    text: String.Format(
                        "User *{0}:{1}* tried to use answer to command Leave.",
                        callbackQuery.Message.From.Id,
                        callbackQuery.Message.From.Username)
                );
                return;
            }

            Manager.BotClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

            string data = callbackQuery.Data.Replace("/Leave ", "");
            if(data == "yes")
            {
                //MessageQueueManager.EnqueueMessage(
                //    new ChatMessage()
                //    {
                //        Timestamp = DateTime.UtcNow,
                //        Chat = callbackQuery.Message.Chat,
                //        ParseMode = ParseMode.Markdown,
                //        Text = "Well, I hope to see you soon...\n\nGood bye! 👋🏼"
                //    });

                Manager.BotClient.SendTextMessageAsync(
                       chatId: callbackQuery.Message.Chat.Id,
                       parseMode: ParseMode.Markdown,
                       text: "Well, I hope to see you soon...\n\nGood bye! 👋🏼"
                   );

                Manager.BotClient.SendTextMessageAsync(
                    chatId: CacheData.ControlChatId,
                    parseMode: ParseMode.Markdown,
                    text: $"Chat Id {callbackQuery.Message.Chat.Id} left due to command /leave");

                System.Threading.Thread.Sleep(1000); // Wait that the goodbye message is sent
                Manager.BotClient.LeaveChatAsync(callbackQuery.Message.Chat.Id);
            }
        }
    }
}
