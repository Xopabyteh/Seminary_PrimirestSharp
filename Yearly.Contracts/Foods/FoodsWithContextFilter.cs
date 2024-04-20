namespace Yearly.Contracts.Foods;

public class FoodsWithContextFilter(string nameFilter)
{
    public string NameFilter { get; set; } = nameFilter;
}