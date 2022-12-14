namespace Outreach.HomeTask.Proxx.Domain.Helpers;

public static class RandomHelper
{
    // We can improve this method to include excluding functionality so when we'll generate a board when user clicks on field and this field should be without bomb
    public static HashSet<int> CreateRandomDistributedNumbers(int max, int count)
    {
        List<int> candidates = Enumerable.Range(0, max).ToList();
        Random random = new();
        HashSet<int> result = new();
        while (result.Count < count)
        {
            int i = random.Next(candidates.Count);
            result.Add(candidates[i]);
            candidates.RemoveAt(i);
        }

        return result;
    }
}
