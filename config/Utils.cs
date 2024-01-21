// Copyright (c) Microsoft. All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

// ReSharper disable InconsistentNaming
public static class Utils
{
    // Function used to wrap long lines of text
    public static void LoadEnvFile()
    {
        List<string> possiblePaths = new List<string>
        {
            "../config/.env",
            "config/.env",
            ".env"
            // Add more paths here as needed
        };

        string foundFilePath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                foundFilePath = path;
                break;
            }
        }

        if (foundFilePath == null)
        {
            throw new FileNotFoundException("The .env file could not be found in the specified locations.");
        }

        foreach (var line in File.ReadAllLines(foundFilePath))
        {
            var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                continue; // or handle the error in your preferred way
            }

            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }

    public static string KeyValuePairsStringToJson(string input)
    {
        var keyValuePairs = input.Split(new[] { ", " }, StringSplitOptions.None);

        var data = new Dictionary<string, string>();

        foreach (var pair in keyValuePairs)
        {
            var parts = pair.Split(new[] { ": " }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                data.Add(parts[0].Trim(), parts[1].Trim());
            }
        }

        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        return json;
    }

    public static string WordWrap(string text, int maxLineLength)
    {
        var result = new StringBuilder();
        int i;
        var last = 0;
        var space = new[] { ' ', '\r', '\n', '\t' };
        do
        {
            i = last + maxLineLength > text.Length
                ? text.Length
                : (text.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' }, Math.Min(text.Length - 1, last + maxLineLength)) + 1);
            if (i <= last) i = Math.Min(last + maxLineLength, text.Length);
            result.AppendLine(text.Substring(last, i - last).Trim(space));
            last = i;
        } while (i < text.Length);

        return result.ToString();
    }

    public static string ReadFile(string filePath)
    {
        try
        {
            // Read all text from the file and return it
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            // Handle any exceptions (like file not found, no permissions, etc.)
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }
}
