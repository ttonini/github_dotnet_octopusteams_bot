namespace OctopusBot.Utilities
{
    public static class EmojiConverter
    {
        public static string ConvertStatusToEmoji(string responseStatus)
        {
            string unicodeString;
            if (responseStatus == "Success")
            {
                unicodeString = char.ConvertFromUtf32(0x1F60A) + ' ' + char.ConvertFromUtf32(0x2705);
            }
            else
            {
                unicodeString = char.ConvertFromUtf32(0x1F622) + ' ' + char.ConvertFromUtf32(0x274C);
            }

            return unicodeString;
        }
    }
}