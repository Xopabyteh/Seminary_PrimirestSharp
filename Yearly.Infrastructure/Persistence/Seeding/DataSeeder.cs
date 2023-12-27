using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.WeeklyMenuAgg;

namespace Yearly.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeds a few foods, daily menus and a weekly menu. Photos, similarity table records. Persists the given user adminUser.
/// Uses the current <see cref="IPhotoStorage"/> dependency to upload photo files from <b>wwwroot/seedPhotos/</b>.
/// </summary>
public class DataSeeder
{

    private readonly PrimirestSharpDbContext _context;
    private readonly IPhotoStorage _photoStorage;

    public DataSeeder(PrimirestSharpDbContext context, IPhotoStorage photoStorage)
    {
        _context = context;
        _photoStorage = photoStorage;
    }

    /// <summary>
    /// Always performs db reset and seeds an admin user
    /// </summary>
    public void Seed(
        User adminUser,
        bool seedMenus)
    {
        //Db reset
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        //Seeding
        _context.Users.Add(adminUser);

        if (seedMenus)
        {
            SeedSampleMenu(adminUser);
        }

        //Save
        _context.SaveChanges();
    }


    private void SeedSampleMenu(User admin)
    {
        const int weekMenuId = 1337;

        //Foods
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


        //Seeding photos
        var resourcesBasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "seedPhotos");
        SeedPhotos(
            approvedPhotoResources: new Dictionary<string, Food>
            {
                {Path.Combine(resourcesBasePath, "Famine.jpg"), foods[1]},
                {Path.Combine(resourcesBasePath, "Famine2.jpg"), foods[1]}
            },
            waitingPhotoResources: new Dictionary<string, Food>
            {
                {Path.Combine(resourcesBasePath, "Conquest.jpg"), foods[0]}
            },
            photoApprover: admin);

        //Weekly menu
        var weeklyMenu = WeeklyMenu.Create(new(weekMenuId), new List<DailyMenu>()
        {
            new(foods.Take(3).Select(f => f.Id).ToList(), new DateTime(2023, 12, 26)),
            new(foods.Skip(3).Take(3).Select(f => f.Id).ToList(), new DateTime(2023, 12, 27)),
        });

        //Similarity records
        var similarityRecords = new List<FoodSimilarityRecord>() {new(foods[5].Id, foods[3].Id, 0.9)};

        _context.Foods.AddRange(foods);
        _context.WeeklyMenus.Add(weeklyMenu);
        _context.FoodSimilarityTable.AddRange(similarityRecords);
    }

    /// <param name="approvedPhotoResources">Key: fileResourceLink (path to file), Value: food to which the photo belongs</param>
    /// <param name="waitingPhotoResources">Key: fileResourceLink (path to file), Value: food to which the photo belongs</param>
    /// <param name="photoApprover"></param>
    private void SeedPhotos(
        IDictionary<string, Food> approvedPhotoResources,
        IDictionary<string, Food> waitingPhotoResources,
        User photoApprover)
    {
        var uploadedPhotos = new List<Photo>();

        foreach (var photoResource in approvedPhotoResources.Concat(waitingPhotoResources))
        {
            //Load file from path
            //Convert to IFormFile
            //Upload

            var file = File.OpenRead(photoResource.Key);
            var formFile = new FormFile(file, 0, file.Length, file.Name, file.Name);

            var photoId = new PhotoId(Guid.NewGuid());
            var link = _photoStorage.UploadPhotoAsync(formFile, Photo.NameFrom(photoId, photoResource.Value)).Result.Value;

            //var photo = new Photo(photoId, photoApprover.Id, DateTime.UtcNow, photoResource.Value.Id, link);
            var photo = photoApprover.PublishPhoto(photoId, DateTime.UtcNow, photoResource.Value.Id, link);
            uploadedPhotos.Add(photo);
        }

        foreach (var approvedPhoto in uploadedPhotos.Where(up => approvedPhotoResources.Values.Any(af => af.Id == up.FoodId)))
        {
            photoApprover.ApprovePhoto(approvedPhoto);
        }

        _context.Photos.AddRange(uploadedPhotos);
    }
}