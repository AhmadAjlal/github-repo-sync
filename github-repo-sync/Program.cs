using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("🚀 RepoSyncApp\n");

        string org = GetInput("GitHub Organization Name", "GITHUB_ORG");
        string baseDir = GetInput("Base Directory", "BASE_DIR");
        string token = GetSecret("GitHub Token", "GITHUB_TOKEN");

        Directory.CreateDirectory(baseDir);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("RepoSyncApp");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var repos = await GetAllRepos(client, org);

            Console.WriteLine($"\n📦 Found {repos.Count} repositories\n");

            // Parallel processing (safe limit)
            var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            await Task.Run(() =>
            {
                Parallel.ForEach(repos, options, repo =>
                {
                    ProcessRepo(repo, baseDir);
                });
            });

            Console.WriteLine("\n✅ Sync complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fatal error: {ex.Message}");
        }
    }

    static string GetInput(string label, string envKey)
    {
        var env = Environment.GetEnvironmentVariable(envKey);

        if (!string.IsNullOrWhiteSpace(env))
        {
            Console.WriteLine($"{label}: (from ENV)");
            return env;
        }

        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    static string GetSecret(string label, string envKey)
    {
        var env = Environment.GetEnvironmentVariable(envKey);

        if (!string.IsNullOrWhiteSpace(env))
        {
            Console.WriteLine($"{label}: (from ENV)");
            return env;
        }

        Console.Write($"{label}: ");
        string input = "";

        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[0..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return input;
    }

    static void ProcessRepo(Repo repo, string baseDir)
    {
        string repoPath = Path.Combine(baseDir, repo.name);

        try
        {
            if (!Directory.Exists(repoPath))
            {
                Console.WriteLine($"[CLONE] {repo.name}");
                RunGit($"clone {repo.clone_url}", baseDir);
            }
            else
            {
                Console.WriteLine($"[PULL]  {repo.name}");
                RunGit("pull", repoPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {repo.name}: {ex.Message}");
        }
    }

    static async Task<List<Repo>> GetAllRepos(HttpClient client, string org)
    {
        var allRepos = new List<Repo>();
        int page = 1;

        while (true)
        {
            var url = $"https://api.github.com/orgs/{org}/repos?per_page=100&page={page}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GitHub API error: {response.StatusCode}");
                throw new Exception($"GitHub API error: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<Repo>>(json);

            if (repos == null || repos.Count == 0)
                break;

            allRepos.AddRange(repos);
            page++;
        }

        return allRepos;
    }

    static void RunGit(string args, string workingDir)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(output))
            Console.WriteLine(output);

        if (!string.IsNullOrWhiteSpace(error))
            Console.WriteLine(error);
    }

    public class Repo
    {
        public string name { get; set; } = string.Empty;
        public string clone_url { get; set; } = string.Empty;
    }
}