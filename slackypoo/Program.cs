using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ActiveUp.Net.Mail;
using slackypoo.Helpers;
using slackypoo.Managers;
using slackypoo.Utils;

namespace slackypoo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // get gmail messages
            var messages = ReadImapEmail();

            // load previously written Ids
            FileManager.GetData();

            // This initialization mess is to avoid links to the same referenced in-memory objects... 
            // It is necessary, or the system will not work properly
            var currentEmailIds = Globals.Current.CurrentEmailIds;
            var enumerable = messages as Message[] ?? messages.ToArray();
            var allEmailIdsArray = new string[enumerable.Count() + currentEmailIds.Count];
            currentEmailIds.CopyTo(allEmailIdsArray);
            var allEmailIds = allEmailIdsArray.Where(a => a != null).ToList();

            foreach (var message in enumerable)
            {
                if (currentEmailIds.Count == 0)
                {
                    TransferEmailToSlack(message);
                    allEmailIds.Add(message.MessageId);
                }
                else
                {
                    var isInEmailList = currentEmailIds.Any(a => a == message.MessageId);

                    // email content was not sent to slack already
                    if (!isInEmailList)
                    {
                        TransferEmailToSlack(message);
                        allEmailIds.Add(message.MessageId);
                    }
                }
            }

            // write new current ids to global ids
            Globals.Current.CurrentEmailIds = allEmailIds;

            // save new global ids to disk
            FileManager.SaveDataToDisk();

            // test slack write
            //var messagetTest= $"{DateTime.Now}: Test API generated message to {ConfigurationManager.AppSettings.Get("SlackChannel")}";
            //SlackWriteMessage(messagetTest);
        }

        private static void TransferEmailToSlack(Message message)
        {
            //SlackWriteMessage(HtmlHelper.StripHtml(System.Web.HttpUtility.HtmlDecode(message.Summary)));

            var indexStartBuildUri = message.BodyHtml.Text.IndexOf("href=\"http", StringComparison.Ordinal) + 6;
            var indexEndBuildUri = message.BodyHtml.Text.Substring(indexStartBuildUri).IndexOf("\"", StringComparison.Ordinal);
            var targetBuildUri = message.BodyHtml.Text.Substring(indexStartBuildUri, indexEndBuildUri);
            var warningBuildPartialSuccess = message.Subject.Contains("partially");
            var goodBuildOrBadBuild = message.Subject.Contains("succeeded") ? ":white_check_mark:" : ":x:";

            if (warningBuildPartialSuccess)
            {
                // partial success case
                SlackWriteMessage(
                    Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EmailSendSubjectOnly"))
                        ? $"{":warning:"} {DateTime.Now}: <{HttpUtility.HtmlDecode(targetBuildUri)}|{message.Subject}>"
                        : $"{DateTime.Now}: {message.Summary}");
            }
            else
            {
                // full success or failure case
                SlackWriteMessage(
                    Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EmailSendSubjectOnly"))
                        ? $"{goodBuildOrBadBuild} {DateTime.Now}: <{HttpUtility.HtmlDecode(targetBuildUri)}|{message.Subject}>"
                        : $"{DateTime.Now}: {message.Summary}");
            }

            Console.WriteLine($"{DateTime.Now}: Test API generated message with MessageId {message.MessageId} to {ConfigurationManager.AppSettings.Get("SlackChannel")}");
        }

        private static void SlackWriteMessage(string message)
        {
            var client = new SlackClientAPI();
            var token = ConfigurationManager.AppSettings.Get("SlackApiToken");

            // Slack message formatting here: https://api.slack.com/docs/message-formatting
            // Example: argument parse: full or none
            var p = new Arguments
            {
                Channel = ConfigurationManager.AppSettings.Get("SlackChannel"),
                Username = ConfigurationManager.AppSettings.Get("SlackUsername"),
                Text = message,
                Token = token,
                Parse = "none"
            };

            var r = client.PostMessage(p);

            if (!r.Ok)
            {
                Console.Write("Error: {0}", r.Error);
                //Console.Read();
            }
        }

        public static IEnumerable<Message> ReadImapEmail()
        {
            var mailRepository = new MailRepository(
                                    ConfigurationManager.AppSettings.Get("EmailServer"),
                                    Convert.ToInt32(ConfigurationManager.AppSettings.Get("EmailPort")),
                                    Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EmailUseSsl")),
                                    ConfigurationManager.AppSettings.Get("EmailUser"),
                                    ConfigurationManager.AppSettings.Get("EmailPassword")
                                );

            //var emailList = mailRepository.GetAllMails("inbox");
            //var emailList = mailRepository.GetMailsBySearch(ConfigurationManager.AppSettings.Get("EmailMailbox"), "jax");
            var emailList = mailRepository.GetUnreadMails("inbox");

            var readImapEmail = emailList as IList<Message> ?? emailList.ToList();
            foreach (Message email in readImapEmail)
            {
                Console.WriteLine("<p>{0}: {1}</p><p>{2}</p>", email.From, email.Subject, email.BodyHtml.Text);
                if (email.Attachments.Count > 0)
                {
                    foreach (MimePart attachment in email.Attachments)
                    {
                        Console.WriteLine("<p>Attachment: {0} {1}</p>", attachment.ContentName, attachment.ContentType.MimeType);
                    }
                }
            }

            return readImapEmail;
        }
    }
}
