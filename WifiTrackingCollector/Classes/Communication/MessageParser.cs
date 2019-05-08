namespace DataCollector
{
    public static class MessageParser
    {
        public static string Parse(string message)
        {
            string parsed = "";
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '&')
                {
                    for (int j = i + 1; j < message.Length; j++)
                    {
                        if (message[j] == '%')
                        {
                            return parsed;
                        }
                        parsed += message[j];
                    }
                }
            }

            return message;
        }

        public static string Encode(string message)
        {
            return "&" + message + "%";
        }

        public static string GetValue(string message)
        {
            try
            {
                string[] split = message.Split(":");
                if (split[1] != null && split.Length == 2)
                {
                    return split[1];
                }
            }
            catch
            {
                //TODO
            }

            return null;
        }
    }
}
