using Bogus;

namespace FileGenerator;

public class LargeFileGenerator
{
    private readonly HashSet<string> _vocabulary;
    private readonly ulong _numberOfLines;
    private readonly short _vocabularyCapacity;
    private readonly string _fullPath;

    private LargeFileGenerator(string fullPath, ulong numberOfLines, short vocabularyCapacity)
    {
        _vocabularyCapacity = vocabularyCapacity;
        _vocabulary = new HashSet<string>(vocabularyCapacity);
        _fullPath = fullPath;
        _numberOfLines = numberOfLines;
    }
    
    public void Generate()
    {
        FillVocabulary();

        using (var writer = new StreamWriter(_fullPath))
        {
            var random = new Random();
            for (ulong i = 0; i < _numberOfLines; i++)
            {
                var number = random.Next(0, _vocabulary.Count);
                var word = _vocabulary.ElementAt(random.Next(0, _vocabulary.Count - 1));
            
                writer.WriteLine(string.Concat(number.ToString(), ". ", word));
            }
        }
    }

    private void FillVocabulary()
    {
        var faker = new Faker();
        var random = new Random();
        var iterationsRest = short.MaxValue * 4;
        while (true)
        {
            if (random.Next(0, 1000) > 500)
            {
                _vocabulary.Add(faker.Lorem.Sentence().TrimEnd('.'));
            }
            else
            {
                _vocabulary.Add(faker.Lorem.Word());
            }
            --iterationsRest;

            if (_vocabulary.Count == _vocabularyCapacity || iterationsRest == 0)
            {
                break;
            }
        }
    }

    public static LargeFileGenerator Create(string fullPath, ulong numberOfLines, short vocabularyCapacity = 150)
    {
        return new LargeFileGenerator(fullPath, numberOfLines, vocabularyCapacity);
    }
}