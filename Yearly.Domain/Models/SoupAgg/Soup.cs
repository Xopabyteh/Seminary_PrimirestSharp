//using Yearly.Domain.Models.FoodAgg.ValueObjects;
//using Yearly.Domain.Models.PhotoAgg.ValueObjects;

//namespace Yearly.Domain.Models.SoupAgg;

//public class Soup : AggregateRoot<FoodId>
//{
//    public FoodId? AliasForFoodId { get; private set; }

//    private readonly List<PhotoId> _photoIds;
//    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();

//    public string Name { get; private set; }

//    private Soup(
//        FoodId id,
//        List<PhotoId> photos,
//        string name)
//        : base(id)
//    {
//        _photoIds = photos;
//        Name = name;
//    }

//    public static Soup Create(string name)
//    {
//        return new(
//            new FoodId(Guid.NewGuid()),
//            new List<PhotoId>(),
//            name);
//    }


//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//    private Soup() //For EF Core
//#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//        : base(null!)
//    {
        
//    }
//}