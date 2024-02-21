

namespace ML_START_1;

internal static class StoryTeller
{
    private static List<string> _story = new List<string>();

    public static List<string> Story {  get => _story; }

    public static void AddSentence(string sentence) => _story.Add(sentence);

    public static void RemoveSentence(string sentence) => _story.Remove(sentence);

    public static void Clear()
    { 
        _story.Clear(); 
    }

    public static void Tell(int delayInMilliseconds)
    {
        foreach (string sentence in _story)
        {
            Console.WriteLine(sentence);
            Thread.Sleep(delayInMilliseconds);
        }
    }
}
