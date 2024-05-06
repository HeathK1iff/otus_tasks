using Microsoft.Extensions.Configuration;
using otus_tasks.Utils;
using System.Diagnostics;

namespace otus_tasks;

public class Program
{
    public const string QuitKey = "Quit";
    public const string ConfigIni = "app.ini";

    private readonly IConfiguration _configuration;
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddIniFile(ConfigIni);
        IConfiguration configuration = builder.Build();

        var program = new Program(configuration);
        program.Run();
    }

    public Program(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Run()
    {
        PrintHeader();

        string answer;
        while (!(answer = Ask(">")).Equals(QuitKey, StringComparison.InvariantCultureIgnoreCase))
        {
            int selected = int.TryParse(answer, out selected) ? selected : -1;

            if (selected <= 0)
            {
                Print("Incorrect input");
                continue;
            }

            switch (selected)
            {
                case 1:
                    ActionWithParallelTasks();
                    break;
                case 2:
                    ActionWithoutParallel();
                    break;
                case 3:
                    ActionCountSpacesInFilesForFolderParallel();
                    break;
                default:
                    Print("Incorrect input");
                    break;
            }
        }
    }

    public void PrintHeader()
    {
        Print("Otus task example");
        Print("Please select:");
        Print("1. Run count spaces in parallel mode for (TextFile1.txt, TextFile2.txt, TextFile3.txt) files");
        Print("2. Run count spaces in simple mode for (TextFile1.txt, TextFile2.txt, TextFile3.txt) files");
        Print("3. Count spaces in files for selected folder");
        Print("Quit. Exit program");
    }

    public string Ask(string promt)
    {
        Console.Write(promt);
        return Console.ReadLine();
    }

    public void Print(string message)
    {
        Console.WriteLine(message);
    }

    public void ActionWithParallelTasks()
    {
        Print("Start parallel mode");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var tasks = new Task<int>[]
        {
            WhiteSpaceCounter.CountInFileAsync("TextFile1.txt"),
            WhiteSpaceCounter.CountInFileAsync("TextFile2.txt"),
            WhiteSpaceCounter.CountInFileAsync("TextFile3.txt")
        };

        Task.WhenAll(tasks);

        stopwatch.Stop();

        Print($"Result:{tasks.Sum(t => t.Result)}");

        Print($"Elapsed time (in parallel mode): {stopwatch.ElapsedMilliseconds} ms");
    }

    public void ActionWithoutParallel()
    {
        Print("Start simple mode");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            int taskResult1 = WhiteSpaceCounter.CountInFile("TextFile1.txt");
            int taskResult2 = WhiteSpaceCounter.CountInFile("TextFile2.txt");
            int taskResult3 = WhiteSpaceCounter.CountInFile("TextFile3.txt");
            Print($"Result:{taskResult1 + taskResult2 + taskResult3}");
        }
        finally
        {
            stopwatch.Stop();
        }

        Print($"Elapsed time (in simple mode): {stopwatch.ElapsedMilliseconds} ms");
    }

    public void ActionCountSpacesInFilesForFolderParallel()
    {
        Print("Start parallel mode for folder");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            string path = GetPathFromConfigurationOrCurrent();
            Print($"Path: {path}");
            Console.WriteLine($"Spaces: {WhiteSpaceCounter.CountInFolder(path)} ");
        }
        finally
        {
            stopwatch.Stop();
        }
        Print($"Elapsed time (in parallel mode for folder): {stopwatch.ElapsedMilliseconds} ms");
    }

    public string GetPathFromConfigurationOrCurrent()
    {
        var folder = _configuration["path"];

        if (string.IsNullOrWhiteSpace(folder))
        {
            return Directory.GetCurrentDirectory();
        }

        return folder;
    }
}