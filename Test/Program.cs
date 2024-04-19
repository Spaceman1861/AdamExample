// See https://aka.ms/new-console-template for more information

using Xunit;

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

public sealed record RouteBranch(string Path)
{
    public Dictionary<string, RouteBranch> Branches { get; } = new ();
    public string? Result { get; set; }
}

public class RouterTests
{
    [Fact]
    public void SimpleTest()
    {
        var router = new Router();
        router.AddRoute("simple", "result");

        Assert.Equal("result", router.Resolve("simple"));    
    }

    [Fact]
    public void NotFound()
    {
        var router = new Router();

        Assert.Equal(Router.NotFound, router.Resolve("simple"));
    }

    [Fact]
    public void MultiLevel()
    {
        var router = new Router();
        router.AddRoute("simple/boo", "result");

        Assert.Equal(Router.NotFound, router.Resolve("simple"));
        Assert.Equal("result", router.Resolve("simple/boo"));
    }

    [Fact]
    public void WildCardTest()
    {
        var router = new Router();
        router.AddRoute("simple/*/boo", "result");

        Assert.Equal(Router.NotFound, router.Resolve("simple"));
        Assert.Equal(Router.NotFound, router.Resolve("simple/boo"));
        Assert.Equal("result", router.Resolve("simple/HOTDAWG/boo"));
        Assert.Equal("result", router.Resolve("simple/HOTDAWG12312/boo"));
        Assert.Equal("result", router.Resolve("simple/HOTDAWGasdfawsdf12312/boo"));
    }

    [Fact]
    public void WildCardTest2()
    {
        var router = new Router();
        router.AddRoute("notSoSimple/*/*/*/*/*/*/boo", "result");

        Assert.Equal(Router.NotFound, router.Resolve("notSoSimple"));
        Assert.Equal(Router.NotFound, router.Resolve("notSoSimple/boo"));
        Assert.Equal(Router.NotFound, router.Resolve("notSoSimple/HOTDAWG/boo"));
        Assert.Equal(Router.NotFound, router.Resolve("notSoSimple/HOTDAWG12312/boo"));
        Assert.Equal(Router.NotFound, router.Resolve("notSoSimple/HOTDAWGasdfawsdf12312/boo"));
        Assert.Equal("result", router.Resolve("notSoSimple/1/2/3/4/5/6/boo"));
    }
}   