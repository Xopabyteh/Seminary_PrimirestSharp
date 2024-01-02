﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Reflection;
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
    private const string menuSeedJson = """
                                    {
                                        "weeklyMenus": [
                                            {
                                                "dailyMenus": [
                                                    {
                                                        "date": "2024-01-03T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Kuřecí maso na kari se smetanou, jasmínová rýže",
                                                                "allergens": "1a,7",
                                                                "photoLinks": [],
                                                                "foodId": "b6f908fc-9215-4505-b6d9-4e4ab3f46efe",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450545,
                                                                    "itemId": 131450551
                                                                }
                                                            },
                                                            {
                                                                "name": "Zeleninový kuskus se sýrem, přízdoba",
                                                                "allergens": "1a,7",
                                                                "photoLinks": [],
                                                                "foodId": "8c317811-62ac-453e-861d-4f95cfa014db",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450545,
                                                                    "itemId": 131450550
                                                                }
                                                            },
                                                            {
                                                                "name": "Vepřová pečeně na bylinkách, vařené brambory",
                                                                "allergens": "1a,7,9,13",
                                                                "photoLinks": [],
                                                                "foodId": "407b0a50-6715-418d-9d41-b507e26a4810",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450545,
                                                                    "itemId": 131450552
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-04T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Kuřecí paličky pečené na medu, dušená rýže",
                                                                "allergens": "1d,6,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "7c1923dc-5682-4423-9157-1a2892941a9b",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450553,
                                                                    "itemId": 131450559
                                                                }
                                                            },
                                                            {
                                                                "name": "Čočkové placičky, salát Coleslaw",
                                                                "allergens": "1d,3,7,9,10,13",
                                                                "photoLinks": [],
                                                                "foodId": "81f0e73f-9202-42ce-89d0-406cd5a86b78",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450553,
                                                                    "itemId": 131450560
                                                                }
                                                            },
                                                            {
                                                                "name": "Frankfurtská hovězí pečeně, domácí houskové knedlíky",
                                                                "allergens": "1a,1c,1d,3,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "f312975e-91e0-4a3b-9ce4-4dedab1a3430",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450553,
                                                                    "itemId": 131450558
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-05T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Sekaná pečeně, bramborová kaše, přízdoba",
                                                                "allergens": "1a,1c,3,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "fb37af0b-e698-44ca-8f9c-6589834c21b4",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450561,
                                                                    "itemId": 131450567
                                                                }
                                                            },
                                                            {
                                                                "name": "Rýžová kaše s máslem a kakaem, ovocný kompot",
                                                                "allergens": "1a,6,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "8405020b-22fa-444b-8ccc-a453f96b762a",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450561,
                                                                    "itemId": 131450566
                                                                }
                                                            },
                                                            {
                                                                "name": "Gnocchi s krůtím masem, sušenými rajčaty, smetanou a sýrem.",
                                                                "allergens": "1a,3,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "66bd4ce0-5129-4468-a0d6-d5185352a87c",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 131450544,
                                                                    "dayId": 131450561,
                                                                    "itemId": 131450568
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ],
                                                "primirestMenuId": 131450544
                                            },
                                            {
                                                "dailyMenus": [
                                                    {
                                                        "date": "2024-01-08T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Smažený květák, vařené brambory, domácí tatarská omáčka",
                                                                "allergens": "1a,1c,3,4,7,9,10,12",
                                                                "photoLinks": [],
                                                                "foodId": "025f628d-fa5f-4536-aaa0-972d3f8292b9",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262946,
                                                                    "itemId": 133262953
                                                                }
                                                            },
                                                            {
                                                                "name": "Čína z čerstvé zeleniny a praženými nudličkami z kuřecích prsou (mrkev, paprika, pórek, žampióny), jasmínová rýže",
                                                                "allergens": "1a,3,6,9,12",
                                                                "photoLinks": [],
                                                                "foodId": "1408363f-2fec-4cc6-bb10-c4802eda8632",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262946,
                                                                    "itemId": 133262951
                                                                }
                                                            },
                                                            {
                                                                "name": "Vepřové ragú s rajčaty a bylinkami (po Italsku), kuskus",
                                                                "allergens": "1a,9,12",
                                                                "photoLinks": [],
                                                                "foodId": "17cc9c4e-452a-4531-b590-daab7c23558a",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262946,
                                                                    "itemId": 133262952
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-09T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Zapečené brambory se špenátem, sýrem a smetanou, přízdoba",
                                                                "allergens": "1a,1c,3,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "6f8096c8-dff1-49dd-b9dc-240c5185f581",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262954,
                                                                    "itemId": 133262961
                                                                }
                                                            },
                                                            {
                                                                "name": "Hovězí maso vařené, rajská omáčka, vařené těstoviny",
                                                                "allergens": "1a,1c,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "32097992-a25b-46ab-8cf7-5d1abae7b770",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262954,
                                                                    "itemId": 133262959
                                                                }
                                                            },
                                                            {
                                                                "name": "Krůtí prsa s ananasem a karamelizovanou cibulkou, dušená rýže",
                                                                "allergens": "1a,1c,6,9",
                                                                "photoLinks": [],
                                                                "foodId": "8c6ec3f7-6c72-4d2c-8855-e01a8483c608",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262954,
                                                                    "itemId": 133262960
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-10T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Kuřecí steak s mozzarellou a rajčetem (capresse), bulgur",
                                                                "allergens": "1a,1b,7,13",
                                                                "photoLinks": [],
                                                                "foodId": "c69666ae-4fb9-45ad-9be0-18375f0e98f6",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262962,
                                                                    "itemId": 133262968
                                                                }
                                                            },
                                                            {
                                                                "name": "Sumeček africký na másle a bylinkách, vařené brambory, přízdoba",
                                                                "allergens": "1a,1b,4,7",
                                                                "photoLinks": [],
                                                                "foodId": "0c909223-4be7-4de0-b0d5-e33ffa427cc6",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262962,
                                                                    "itemId": 133262967
                                                                }
                                                            },
                                                            {
                                                                "name": "Tarhoňa s grilovanou zeleninou, sýr typu feta",
                                                                "allergens": "1a,1b,7",
                                                                "photoLinks": [],
                                                                "foodId": "e656d088-b9a6-470e-a561-e6eab8ded620",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262962,
                                                                    "itemId": 133262969
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-11T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Špagety aglio olio se sýrem a rukolou",
                                                                "allergens": "1a,3,7,9,12",
                                                                "photoLinks": [],
                                                                "foodId": "2d34f814-40b2-41c8-bc5c-9278396f592a",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262970,
                                                                    "itemId": 133262977
                                                                }
                                                            },
                                                            {
                                                                "name": "Pečené kuřecí stehno, dušená rýže, přízdoba",
                                                                "allergens": "7,9,12,13",
                                                                "photoLinks": [],
                                                                "foodId": "98b896c5-c719-4c17-9761-d94f899f3a55",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262970,
                                                                    "itemId": 133262975
                                                                }
                                                            },
                                                            {
                                                                "name": "Řecká masová musaka s lilkem (zapečené brambory s lilkem a masovou směsí s rajčaty), přízdoba",
                                                                "allergens": "1a,3,7,9,12",
                                                                "photoLinks": [],
                                                                "foodId": "8e9e901b-3f4b-43d2-93d2-dc2c8fa53784",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262970,
                                                                    "itemId": 133262976
                                                                }
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        "date": "2024-01-12T00:00:00",
                                                        "foods": [
                                                            {
                                                                "name": "Brokolicové placičky, bramborová kaše, přízdoba",
                                                                "allergens": "1a,1c,3,7",
                                                                "photoLinks": [],
                                                                "foodId": "bbd8827a-0e4f-4468-84c5-005d447ea5e5",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262978,
                                                                    "itemId": 133262983
                                                                }
                                                            },
                                                            {
                                                                "name": "Vepřová kýta na smetaně, domácí houskové knedlíky",
                                                                "allergens": "1a,3,7,9",
                                                                "photoLinks": [],
                                                                "foodId": "541a95f1-9eb9-435d-be71-558b569dbca5",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262978,
                                                                    "itemId": 133262984
                                                                }
                                                            },
                                                            {
                                                                "name": "Chicken Tikka Masala-kuřecí kousky se směsí indického koření a rýží basmati (drcená rajčata, zázvor, sezam, jogurt)",
                                                                "allergens": "1a,3,7,10,11",
                                                                "photoLinks": [],
                                                                "foodId": "beccb07e-aa95-4545-a497-8b6175f7b034",
                                                                "primirestFoodIdentifier": {
                                                                    "menuId": 133262945,
                                                                    "dayId": 133262978,
                                                                    "itemId": 133262985
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ],
                                                "primirestMenuId": 133262945
                                            }
                                        ]
                                    }
                                    """;

    private readonly PrimirestSharpDbContext _context;
    private readonly IPhotoStorage _photoStorage;

    public DataSeeder(PrimirestSharpDbContext context, IPhotoStorage photoStorage)
    {
        _context = context;
        _photoStorage = photoStorage;
    }

    public void DbReset()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    
    public void SeedAdminUser(User admin)
    {
        //Seeding
        _context.Users.Add(admin);

        _context.SaveChanges();
    }

    public void SeedMenus(User admin)
    {
        // Impl (mostly) by ChatGPT (lol)

        var weeklyMenus = new List<WeeklyMenu>();
        var foods = new List<Food>();

        // Parse the JSON data
        var jsonObject = JObject.Parse(menuSeedJson);
        var weeklyMenusArray = jsonObject["weeklyMenus"];

        foreach (var weeklyMenu in weeklyMenusArray)
        {
            var dailyMenuList = new List<DailyMenu>();
            var primirestMenuId = weeklyMenu["primirestMenuId"].Value<int>();
            var dailyMenusArray = weeklyMenu["dailyMenus"];

            foreach (var dailyMenu in dailyMenusArray)
            {
                var date = dailyMenu["date"].Value<DateTime>();
                var foodsArray = dailyMenu["foods"];

                var foodIds = new List<FoodId>();
                foreach (var food in foodsArray)
                {
                    var name = food["name"].Value<string>();
                    var allergens = food["allergens"].Value<string>();
                    var foodId = Guid.Parse(food["foodId"].Value<string>());

                    var primirestFoodIdentifier = food["primirestFoodIdentifier"];
                    var menuId = primirestFoodIdentifier["menuId"].Value<int>();
                    var dayId = primirestFoodIdentifier["dayId"].Value<int>();
                    var itemId = primirestFoodIdentifier["itemId"].Value<int>();

                    foodIds.Add(new FoodId(foodId));

                    var foodObj = Food.Create(name, allergens, new(menuId, dayId, itemId));

                    //foodObj.Id.Value = foodId;
                    //Override foodObj.Id.Value with reflection
                    var propertyInfo = typeof(FoodId).GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
                    propertyInfo!.SetValue(foodObj.Id, foodId);

                    foods.Add(foodObj);
                }

                var dailyMenuObject = new DailyMenu(foodIds, date);
                dailyMenuList.Add(dailyMenuObject);
            }

            var weeklyMenuObject = WeeklyMenu.Create(new WeeklyMenuId(primirestMenuId), dailyMenuList);
            weeklyMenus.Add(weeklyMenuObject);
        }
        
        //Save
        _context.Foods.AddRange(foods);
        _context.WeeklyMenus.AddRange(weeklyMenus);
        _context.SaveChanges();
    }

    public void SeedFromBible(User admin)
    {
        void SeedPhotosFromBible(
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
        SeedPhotosFromBible(
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

        _context.SaveChanges();
    }
}