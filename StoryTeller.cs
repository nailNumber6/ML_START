

namespace ML_START_1
{
    internal static class StoryTeller
    {
        private static List<string> _story = new List<string>();

        public static void AddSentence(string sentence) => _story.Add(sentence);

        public static void RemoveSentence(string sentence) => _story.Remove(sentence);

        public static void Clear(string sentence) => _story.Clear();

        public static async void Tell(int delay)
        {
            foreach (string sentence in _story)
            {
                Console.WriteLine(sentence);
                await Task.Delay(delay);
            }
        }
    }
}
