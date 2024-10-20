﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using RoadOfGroping.Common.DependencyInjection;
using RoadOfGroping.Common.Helper;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Email
{
    /// <summary>
    /// 邮件通知接口
    /// </summary>
    public interface IEmailPushManager
    {
        Task<string> SendMessage(EmailMessagePushData data);
    }

    /// <summary>
    /// 邮件通知服务
    /// </summary>
    public class EmailPushManager : IEmailPushManager, ITransientDependency
    {
        private readonly ILogger<EmailPushManager> _logger;

        public EmailPushManager(ILogger<EmailPushManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 邮件通知服务
        /// </summary>
        /// <param name="data">邮件通知内容</param>
        public async Task<string> SendMessage(EmailMessagePushData data)
        {
            var emailSettings = AppsettingHelper.AppOption<EmailSettings>("EmailSettings");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.UserName));
            message.To.Add(new MailboxAddress(data.ToName, data.ToEmailAddress));
            message.Subject = data.Subject;

            message.Body = new TextPart("plain")
            {
                Text = data.Content
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(emailSettings.Host, emailSettings.Port, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(emailSettings.UserName, emailSettings.Password);

                    var result = await client.SendAsync(message);
                    client.Disconnect(true);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                throw;
            }
        }
    }
}