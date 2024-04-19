namespace Test;

public sealed record RouteBranch(string Path)
{
    public Dictionary<string, RouteBranch> Branches { get; } = new ();
    public string? Result { get; set; }
}