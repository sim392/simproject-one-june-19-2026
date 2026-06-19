// SearchEngine.cs
using System;
using System.Collections.Generic;

public static class SearchEngine
{
    public static List<ISearchable> Search(List<ISearchable> items, string keyword)
    {
        List<ISearchable> results = new List<ISearchable>();
        foreach (var item in items)
        {
            if (item.MatchesSearch(keyword))
            {
                results.Add(item);
            }
        }
        return results;
    }

    public static void DisplayResults(List<ISearchable> results)
    {
        if (results.Count == 0)
        {
            Console.WriteLine("No results found");
            return;
        }
        Console.WriteLine($"Found {results.Count} results:");
        foreach (var result in results)
        {
            Console.WriteLine($"- {result.GetSearchSummary()}");
        }
    }
}