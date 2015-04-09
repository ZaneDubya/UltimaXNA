using System;
using System.Text;

public static class StringExtensions
{
    public static string Wrap(this string sentence, int limit, int indentationCount, char indentationCharacter)
    {
        var words = sentence.Replace("\n", " ").Replace("\r", " ").Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var counter = 0;
        var builder = new StringBuilder();

        for(int index = 0; index < words.Length; index++)
        {
            var word = words[index];

            if((builder.Length + word.Length) / limit > counter)
            {
                counter++;
                builder.AppendLine();

                for (int i = 0; i < indentationCount; i++)
                {
                    builder.Append(indentationCharacter);
                }
            }

            builder.Append(word);

            if (index < words.Length)
            {
                builder.Append(" ");
            }
        }

        return builder.ToString();
    }
}