// See https://aka.ms/new-console-template for more information

namespace Test;

public sealed class Router
{
    private readonly RouteBranch _routes = new ("");
    private const string Wildcard = "*";
    public const string NotFound = "Not Found";

    private static IEnumerable<string> SplitPathToParts(string path)
        => path.Split(
            '/', 
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

    public void AddRoute(string path, string result)
    {
        // Split the path into parts
        var parts = SplitPathToParts(path);

        var currentNode = _routes;
        foreach (var part in parts)
        {
            // If we don't have a branch for this part, create one
            if (!currentNode.Branches.ContainsKey(part))
                currentNode.Branches[part] = new RouteBranch(part);

            currentNode = currentNode.Branches[part];
        }

        // Once we are at the end of the path, set the result
        currentNode.Result = result;
    }

    public string Resolve(string path)
    {
        // Split the path into parts
        var parts = SplitPathToParts(path);

        var currentNode = _routes;
        foreach (var part in parts)
        {
            // Prefer wild card over specific path (for no reason other than we like chaos)
            if (currentNode.Branches.ContainsKey(Wildcard))
                currentNode = currentNode.Branches[Wildcard];
            else if (currentNode.Branches.ContainsKey(part))
                currentNode = currentNode.Branches[part];
            else 
                return NotFound;
        }

        // Once we are at the end of the path, return the result
        return currentNode.Result ?? NotFound;
    }
}