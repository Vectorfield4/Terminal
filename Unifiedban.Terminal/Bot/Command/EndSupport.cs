﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Unifiedban.Terminal.Bot.Command
{
    public class EndSupport : ICommand
    {
        public void Execute(Message message)
        {
            if (!Utils.BotTools.IsUserOperator(message.From.Id, Models.Operator.Levels.Basic))
            {
                MessageQueueManager.EnqueueMessage(
                    new ChatMessage()
                    {
                        Timestamp = DateTime.UtcNow,
                        Chat = message.Chat,
                        Text = CacheData.GetTranslation("en", "feedback_command_error_notoperator")
                    });
                return;
            }

            CacheData.Groups.TryGetValue(message.Chat.Id, out Models.Group.TelegramGroup group);
            if (CacheData.ActiveSupport.Contains(group.GroupId))
            {
                CacheData.ActiveSupport.Remove(group.GroupId);

                Manager.BotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    parseMode: ParseMode.Markdown,
                    text: String.Format(
                        "Operator *{0}* ended the support session.",
                        message.From.Username)
                );
                Manager.BotClient.SendTextMessageAsync(
                    chatId: Convert.ToInt64(CacheData.SysConfigs
                            .Single(x => x.SysConfigId == "ControlChatId")
                            .Value),
                    parseMode: ParseMode.Markdown,
                    text: String.Format(
                        "Operator *{0}:{1}* ended support in *{2}:[{3}]({4})*",
                        message.From.Id,
                        message.From.Username,
                        message.Chat.Id,
                        message.Chat.Title,
                        "https://t.me/" + message.Chat.Username)
                );
            }
        }

        public void Execute(CallbackQuery callbackQuery) { }
    }
}
