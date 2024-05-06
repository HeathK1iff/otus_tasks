using System.Collections.Concurrent;
using System.Diagnostics;

namespace otus_tasks.Utils;

public static class WhiteSpaceCounter
{
    private static long DefaultBufferSize = 1024;

    public static int CountInFolder(string path)
    {
        ConcurrentBag<int> intBags = new ConcurrentBag<int>();
        Parallel.ForEach<string>(Directory.GetFiles(path), fileName =>
        {
            intBags.Add(CountInFile(fileName));
        });

        return intBags.Sum();
    }

    #region CountInFile
    public static Task<int> CountInFileAsync(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File is not found: {path}");
        }

        return Task.Run(() =>
        {
            return CountInFile(path);
        });       
    }

    public static int CountInFile(string path)
    {
        using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            int count = Count(file);
            Debug.WriteLine($"Whitespace count for {path} : {count}");
            return count;
        }
    }
    #endregion

    #region Count
    public static Task<int> CountAsync(Stream stream)
    {
        return Task.Run(() =>
        {
            return Count(stream);
        });
    }

    public static int Count(Stream stream)
    {
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            int count = 0;
            var buffer = new char[DefaultBufferSize];

            while (reader.Read(buffer, 0, buffer.Length) > 0)
            {
                count += buffer.Count(f => char.IsWhiteSpace(f));
            }
            return count;
        }
    }
    #endregion
}
