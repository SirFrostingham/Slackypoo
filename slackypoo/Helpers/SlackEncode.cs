using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slackypoo.Helpers
{
    public class SlackEncode
    {
        public static string ProcessMessage(string source)
        {
            var result = string.Empty;

            // Slack: translate HTML entities
            result = source.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            return result;
        }
    }
}
