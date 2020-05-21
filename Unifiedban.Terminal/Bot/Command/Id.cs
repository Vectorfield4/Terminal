﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Unifiedban.Terminal.Bot.Command
{
    public class Id : ICommand
    {
        public void Execute(Message message)
        {
            Manager.BotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            string replyText = "";

            if (message.ReplyToMessage != null)
            {
                if (message.ReplyToMessage.ForwardFrom != null)
                {
                    if (!Utils.BotTools.IsUserOperator(message.ReplyToMessage.ForwardFrom.Id))
                    {
                        replyText = CacheData.GetTranslation("en", "command_id_fromReply_negative");
                        replyText = Utils.Parsers.VariablesParser(
                            replyText.Replace("replyToMessage_from_username", "replyToMessage_forwardFrom_from_username"),
                            message);
                        goto doReply;
                    }
                    else
                    {
                        replyText = CacheData.GetTranslation("en", "command_id_fromReply_positive");
                        replyText = Utils.Parsers.VariablesParser(
                            replyText.Replace("replyToMessage_from_username", "replyToMessage_forwardFrom_from_username"),
                            message);
                        goto doReply;
                    }
                }

                if (!Utils.BotTools.IsUserOperator(message.ReplyToMessage.From.Id))
                {
                    replyText = CacheData.GetTranslation("en", "command_id_fromReply_negative");
                    replyText = Utils.Parsers.VariablesParser(replyText, message);
                    goto doReply;
                }
                else
                {
                    replyText = CacheData.GetTranslation("en", "command_id_fromReply_positive");
                    replyText = Utils.Parsers.VariablesParser(replyText, message);
                    goto doReply;
                }
            }

            if (!Utils.BotTools.IsUserOperator(message.From.Id))
            {
                replyText = CacheData.GetTranslation("en", "command_id_negative");
                replyText = Utils.Parsers.VariablesParser(replyText, message);

                Manager.BotClient.SendTextMessageAsync(
                    chatId: CacheData.ControlChatId,
                    parseMode: ParseMode.Markdown,
                    text: String.Format(
                        "User *{0}:{1}* tried to use command Id.",
                        message.From.Id,
                        message.From.Username)
                );
                goto doReply;
            }


            replyText = CacheData.GetTranslation("en", "command_id_positive");
            replyText = Utils.Parsers.VariablesParser(replyText, message);

            doReply:
            MessageQueueManager.EnqueueMessage(
                   new ChatMessage()
                   {
                       Timestamp = DateTime.UtcNow,
                       Chat = message.Chat,
                       ParseMode = ParseMode.Markdown,
                       Text = replyText
                   });
        }

        public void Execute(CallbackQuery callbackQuery) { }
    }
}
