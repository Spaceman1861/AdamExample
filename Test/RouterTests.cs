using Xunit;

namespace Test;

public sealed class RouterTests
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