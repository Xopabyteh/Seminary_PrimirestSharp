using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Infrastructure.Persistence.Seeding;

public class DataSeeder
{
    private readonly PrimirestSharpDbContext _context;

    public DataSeeder(PrimirestSharpDbContext context)
    {
        _context = context;
    }

    public void Seed(User adminUser)
    {
        //Db reset
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        //Seeding
        _context.Users.Add(adminUser);
        SeedSampleMenu(adminUser);

        //Save
        _context.SaveChanges();
    }


    private void SeedSampleMenu(User admin)
    {
        const int weekMenuId = 1337;

        var foods = new List<Food>()
        {
            Food.Create("Conquest", "", new(weekMenuId, 0, 0)),
            Food.Create("Famine", "1a", new(weekMenuId, 0, 1)),
            Food.Create("War", "2b", new(weekMenuId, 0, 2)),
            Food.Create("Death", "3", new(weekMenuId, 1, 3)),
            Food.Create("Famine2ConfirmedAlias", "5", new(weekMenuId, 1, 4)),
            Food.Create("Death2ConsiderableAlias", "6", new(weekMenuId, 1, 5)),
        };

        foods[4].SetAliasForFood(foods[1]); //Set Famine2 as alias to Famine

        //Add photos to Conquest*1 & Famine*2
        var photos = new List<Photo>()
        {
            new(new PhotoId(Guid.NewGuid()), new UserId(26564871), new DateTime(2023, 12, 25), foods[0].Id,
                "https://static.wikia.nocookie.net/bindingofisaacre_gamepedia/images/c/c2/Boss_Conquest_portrait.png/revision/latest?cb=20210409160654"),
            new(new PhotoId(Guid.NewGuid()), new UserId(26564871), new DateTime(2023, 12, 25), foods[1].Id,
                "https://static.wikia.nocookie.net/bindingofisaacre_gamepedia/images/5/55/Boss_Famine_portrait.png/revision/latest?cb=20210409155632"),
            new(new PhotoId(Guid.NewGuid()), new UserId(26564871), new DateTime(2023, 12, 25), foods[1].Id,
                "https://ih1.redbubble.net/image.5150293064.0880/tst,small,845x845-pad,1000x1000,f8f8f8.jpg"),
        };

        admin.ApprovePhoto(photos[1]); //Approve Famine photo 1st
        admin.ApprovePhoto(photos[2]); //Approve Famine photo 2nd

        var weeklyMenu = WeeklyMenu.Create(new(weekMenuId), new List<DailyMenu>()
        {
            new DailyMenu(foods.Take(3).Select(f => f.Id).ToList(), new DateTime(2023, 12, 26)),
            new DailyMenu(foods.Skip(3).Take(3).Select(f => f.Id).ToList(), new DateTime(2023, 12, 27)),
        });

        var similarityRecords = new List<FoodSimilarityRecord>() {new(foods[3].Id, foods[5].Id, 0.9)};

        _context.Foods.AddRange(foods);
        _context.Photos.AddRange(photos);
        _context.WeeklyMenus.Add(weeklyMenu);
        _context.FoodSimilarityTable.AddRange(similarityRecords);
    }
}