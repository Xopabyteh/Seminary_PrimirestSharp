﻿using ErrorOr;
using Newtonsoft.Json;
using Yearly.Infrastructure.Services.Orders.PrimirestStructures;

namespace Yearly.Infrastructure.Services.Menus;

/// <summary>
/// Use mocked data to be more gentle to Primirest API during development
/// </summary>
public class PrimirestMenuProviderDev : IPrimirestMenuProvider
{
    private const string menusJson = """
                                     [
                                       {
                                         "DailyMenus": [
                                           {
                                             "Date": "2024-02-19T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Zeleninový kuskus se sýrem, přízdoba",
                                                 "Allergens": "1a,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141248842,
                                                   "ItemId": 141249359
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí maso v rajčatové omáčce, vařené těstoviny",
                                                 "Allergens": "1a,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141248842,
                                                   "ItemId": 141249366
                                                 }
                                               },
                                               {
                                                 "Name": "Vepřová kýta na žampiónech, dušená rýže",
                                                 "Allergens": "1a,7,9,13",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141248842,
                                                   "ItemId": 141249367
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Čočková"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-20T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Smažený filet z tmavé tresky, vařené brambory, přízdoba",
                                                 "Allergens": "1a,1c,3,4,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141249683,
                                                   "ItemId": 141249851
                                                 }
                                               },
                                               {
                                                 "Name": "Krůtí ragú na povidlech s kořenovou zeleninou, špecle",
                                                 "Allergens": "1a,3,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141249683,
                                                   "ItemId": 141249854
                                                 }
                                               },
                                               {
                                                 "Name": "Pohanková baba (zapečená pohanka s tofu a bramborami), míchaný zeleninový salát",
                                                 "Allergens": "3,6,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141249683,
                                                   "ItemId": 141249856
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Kuřecí vývar s vaječnou sedlinou"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-21T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Čočkové bolognese špagety, strouhaný sýr",
                                                 "Allergens": "1a,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250027,
                                                   "ItemId": 141250101
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí plátek zapečený nivou, dušená rýže",
                                                 "Allergens": "1a,3,7,9,13",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250027,
                                                   "ItemId": 141250102
                                                 }
                                               },
                                               {
                                                 "Name": "Masový nákyp, vařené brambory, salát z kysaného zelí",
                                                 "Allergens": "1a,3,7,9,10,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250027,
                                                   "ItemId": 141250103
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Zeleninová s krupicí a vejcem"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-22T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Pražská hovězí pečeně, domácí houskové knedlíky",
                                                 "Allergens": "1a,1c,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250225,
                                                   "ItemId": 141250391
                                                 }
                                               },
                                               {
                                                 "Name": "Bulgur s kuřecím masem a zeleninou, přízdoba",
                                                 "Allergens": "1a",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250225,
                                                   "ItemId": 141250393
                                                 }
                                               },
                                               {
                                                 "Name": "Cizrnové sauté s hráškem a bylinkami, rýže basmati",
                                                 "Allergens": "1a",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250225,
                                                   "ItemId": 141250395
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Rajčatová polévka s těstovinou"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-23T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Vepřová pečeně na jablkách, bramborová kaše",
                                                 "Allergens": "1a,1c,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250636,
                                                   "ItemId": 141250784
                                                 }
                                               },
                                               {
                                                 "Name": "Krůtí maso na slanině, restovaná tarhoňa",
                                                 "Allergens": "1a,1c,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250636,
                                                   "ItemId": 141250785
                                                 }
                                               },
                                               {
                                                 "Name": "Krémové žampiony se sušenými rajčaty, polenta",
                                                 "Allergens": "1a,1c,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 141248686,
                                                   "DayId": 141250636,
                                                   "ItemId": 141250787
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Frankfurtská s párkem a brambory"
                                             }
                                           }
                                         ],
                                         "PrimirestMenuId": 141248686
                                       },
                                       {
                                         "DailyMenus": [
                                           {
                                             "Date": "2024-02-26T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Kuřecí maso na paprice, vařené těstoviny",
                                                 "Allergens": "1a,3,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357173,
                                                   "ItemId": 144357178
                                                 }
                                               },
                                               {
                                                 "Name": "Vepřové maso dušené na bylinkách, vařené brambory",
                                                 "Allergens": "1a,3,7,9,13",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357173,
                                                   "ItemId": 144357179
                                                 }
                                               },
                                               {
                                                 "Name": "Zeleninové Ratatouille s cizrnou (dušené papriky, lilek a cuketa, rajčata a cizrna), kuskus",
                                                 "Allergens": "1a,3,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357173,
                                                   "ItemId": 144357180
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Pórková s vejci"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-27T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Zapečený květák sýrem a vejci, šťouchané brambory s cibulkou, přízdoba",
                                                 "Allergens": "1a,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357181,
                                                   "ItemId": 144357186
                                                 }
                                               },
                                               {
                                                 "Name": "Paprikový lusk plněný v rajské omáčce, vařené těstoviny",
                                                 "Allergens": "1a,1c,3,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357181,
                                                   "ItemId": 144357187
                                                 }
                                               },
                                               {
                                                 "Name": "Rizoto s trhaným, králičím masem a zeleninou, strouhaný sýr",
                                                 "Allergens": "1a,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357181,
                                                   "ItemId": 144357188
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Zeleninový vývar s těstovinami"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-28T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Hovězí guláš, domácí houskové knedlíky",
                                                 "Allergens": "1a,3,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357189,
                                                   "ItemId": 144357194
                                                 }
                                               },
                                               {
                                                 "Name": "Grilované kuřecí stehno, šťouchané brambory s jarní cibulkou, přízdoba",
                                                 "Allergens": "1a,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357189,
                                                   "ItemId": 144357195
                                                 }
                                               },
                                               {
                                                 "Name": "Ragú z červené čočky s mrkví, bulgur",
                                                 "Allergens": "1a,7",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357189,
                                                   "ItemId": 144357196
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Špenátová"
                                             }
                                           },
                                           {
                                             "Date": "2024-02-29T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Rybí filé (tilápie) pečená na másle, bramborová kaše, přízdoba",
                                                 "Allergens": "1a,4,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357197,
                                                   "ItemId": 144357202
                                                 }
                                               },
                                               {
                                                 "Name": "Chicken Tikka Masala-kuřecí kousky se směsí indického koření a rýží basmati, rýže basmati",
                                                 "Allergens": "1a,7,9,10,11",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357197,
                                                   "ItemId": 144357203
                                                 }
                                               },
                                               {
                                                 "Name": "Nudle s mákem, máslový přeliv",
                                                 "Allergens": "1a,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357197,
                                                   "ItemId": 144357204
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Zeleninová zapražená"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-01T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Koprová omáčka, vařené vejce, vařené brambory",
                                                 "Allergens": "1a,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357205,
                                                   "ItemId": 144357210
                                                 }
                                               },
                                               {
                                                 "Name": "Dušená vepřová kýta s rajčaty, paprikami a žampióny (Novohradská), vařené těstoviny",
                                                 "Allergens": "1a,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357205,
                                                   "ItemId": 144357211
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí plátek na koření \"Gyros\", dušená rýže, přízdoba",
                                                 "Allergens": "7,9,13",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 144357172,
                                                   "DayId": 144357205,
                                                   "ItemId": 144357212
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Ukrajinský boršč"
                                             }
                                           }
                                         ],
                                         "PrimirestMenuId": 144357172
                                       },
                                       {
                                         "DailyMenus": [
                                           {
                                             "Date": "2024-03-04T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Krůtí kung pao (arašídy, sojová omáčka, jarní cibulka), dušená rýže",
                                                 "Allergens": "1a,1d,5,6,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494129,
                                                   "ItemId": 146494134
                                                 }
                                               },
                                               {
                                                 "Name": "Vepřová pečeně, vařené brambory, dušená brokolice s karotkou na másle",
                                                 "Allergens": "1a,1d,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494129,
                                                   "ItemId": 146494135
                                                 }
                                               },
                                               {
                                                 "Name": "Guláš z hlívy s barevnou paprikou, chléb",
                                                 "Allergens": "1a,1b,1d,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494129,
                                                   "ItemId": 146494136
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Rajčatová s ovesnými vločkami"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-05T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Hovězí znojemská pečeně, domácí houskové knedlíky",
                                                 "Allergens": "1a,3,7,9,10",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494137,
                                                   "ItemId": 146494142
                                                 }
                                               },
                                               {
                                                 "Name": "Střapačky se zelím a uzeným masem",
                                                 "Allergens": "1a,7,9,10,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494137,
                                                   "ItemId": 146494143
                                                 }
                                               },
                                               {
                                                 "Name": "Kari květák s červenou čočkou, rýže basmati",
                                                 "Allergens": "7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494137,
                                                   "ItemId": 146494144
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Křimická zelná se zeleninou"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-06T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Květákové placky se sýrem, bramborová kaše, přízdoba",
                                                 "Allergens": "1a,1d,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494146,
                                                   "ItemId": 146494151
                                                 }
                                               },
                                               {
                                                 "Name": "Kořeněný sumeček africký s rajčatovou kari omáčkou, jasmínová rýže",
                                                 "Allergens": "1a,4,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494146,
                                                   "ItemId": 146494152
                                                 }
                                               },
                                               {
                                                 "Name": "Těstoviny se sýrovou omáčkou, mandlemi, kuřecím masem, sypané sýrem, zdobené rukolou",
                                                 "Allergens": "1a,3,7,8a,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494146,
                                                   "ItemId": 146494153
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Veganská Minestrone"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-07T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Kuřecí rizoto se zeleninou, strouhaný sýr, přízdoba",
                                                 "Allergens": "1a,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494157,
                                                   "ItemId": 146494162
                                                 }
                                               },
                                               {
                                                 "Name": "Záhorácký závitek z vepřového masa, plněný kysaným zelím, šťouchané brambory s cibulkou",
                                                 "Allergens": "1a,9,10,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494157,
                                                   "ItemId": 146494163
                                                 }
                                               },
                                               {
                                                 "Name": "Smetanové penne s rajčaty a špenátem",
                                                 "Allergens": "1a,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494157,
                                                   "ItemId": 146494164
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Hovězí vývar s masem a vlasovými nudlemi"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-08T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Sekaná pečeně, vařené brambory, přízdoba",
                                                 "Allergens": "1a,1c,3,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494167,
                                                   "ItemId": 146494172
                                                 }
                                               },
                                               {
                                                 "Name": "Drůbeží játra na cibulce, dušená rýže",
                                                 "Allergens": "1a,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494167,
                                                   "ItemId": 146494173
                                                 }
                                               },
                                               {
                                                 "Name": "Restovaná asijská zelenina s tofu, rýžové nudle",
                                                 "Allergens": "4,6,9,11,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 146494128,
                                                   "DayId": 146494167,
                                                   "ItemId": 146494174
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Jarní zeleninová polévka"
                                             }
                                           }
                                         ],
                                         "PrimirestMenuId": 146494128
                                       },
                                       {
                                         "DailyMenus": [
                                           {
                                             "Date": "2024-03-11T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Penne Napolitana, sypané sýrem (rajčata, česnek, bylinky)",
                                                 "Allergens": "1a,3,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185210,
                                                   "ItemId": 148185215
                                                 }
                                               },
                                               {
                                                 "Name": "Vepřová krkovice pečená na česneku, vařené brambory, přízdoba",
                                                 "Allergens": "1a,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185210,
                                                   "ItemId": 148185216
                                                 }
                                               },
                                               {
                                                 "Name": "Smažená rýže s kuřecím masem a zázvorem",
                                                 "Allergens": "1a,3,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185210,
                                                   "ItemId": 148185217
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Zeleninová s bulgurem"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-12T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Jablková žemlovka, kompot",
                                                 "Allergens": "1a,1c,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185220,
                                                   "ItemId": 148185225
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí maso se zeleninou, teriyiaki omáčkou (sezam, mrkev, zelí, brokolice, jarní cibulka), rýžové nudle",
                                                 "Allergens": "1a,1c,4,6,9,11,14",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185220,
                                                   "ItemId": 148185226
                                                 }
                                               },
                                               {
                                                 "Name": "Smažený máslový řízek v sezamové strouhance, bramborová kaše",
                                                 "Allergens": "1a,1c,3,7,9,11",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185220,
                                                   "ItemId": 148185227
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Zahradnická polévka s rýží"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-13T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Vepřový paprikáš, dušená rýže",
                                                 "Allergens": "1a,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185234,
                                                   "ItemId": 148185239
                                                 }
                                               },
                                               {
                                                 "Name": "Krůtí nudličky \"Chow Mein\"",
                                                 "Allergens": "1a,4,6,7,9,14",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185234,
                                                   "ItemId": 148185240
                                                 }
                                               },
                                               {
                                                 "Name": "Celerový řízek s jogurtovým dipem, vařené brambory",
                                                 "Allergens": "1a,1c,1d,3,7,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185234,
                                                   "ItemId": 148185241
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Ukrajinský boršč"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-14T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Hrachová kaše s cibulkou, vařené vejce, chléb, přízdoba",
                                                 "Allergens": "1a,1b,3,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185242,
                                                   "ItemId": 148185247
                                                 }
                                               },
                                               {
                                                 "Name": "Vepřové výpečky, dušené červené zelí, bramborové knedlíky",
                                                 "Allergens": "1a,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185242,
                                                   "ItemId": 148185248
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí pilaf (rýže, kuřecí maso, zelenina, rajčata)",
                                                 "Allergens": "1a,9,12",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185242,
                                                   "ItemId": 148185249
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Slepičí vývar s masem, nudlemi a zeleninou"
                                             }
                                           },
                                           {
                                             "Date": "2024-03-15T00:00:00Z",
                                             "Foods": [
                                               {
                                                 "Name": "Masové kuličky s rajčatovou omáčkou, vařené těstoviny",
                                                 "Allergens": "1a,1c,3,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185253,
                                                   "ItemId": 148185258
                                                 }
                                               },
                                               {
                                                 "Name": "Kuřecí plátek na koření \"Gyros\", šťouchané brambory s jarní cibulkou, přízdoba",
                                                 "Allergens": "1a,7,9,13",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185253,
                                                   "ItemId": 148185259
                                                 }
                                               },
                                               {
                                                 "Name": "Vaječná frittata s houbami a zeleninou (rajčata, žampióny, paprika, brambory), přízdoba",
                                                 "Allergens": "1a,3,7,9",
                                                 "PrimirestFoodIdentifier": {
                                                   "MenuId": 148185209,
                                                   "DayId": 148185253,
                                                   "ItemId": 148185260
                                                 }
                                               }
                                             ],
                                             "Soup": {
                                               "Name": "Mrkvová s těstovinami"
                                             }
                                           }
                                         ],
                                         "PrimirestMenuId": 148185209
                                       }
                                     ]
                                     """;
    public Task<ErrorOr<List<PrimirestWeeklyMenu>>> GetMenusThisWeekAsync()
        => Task.FromResult((ErrorOr<List<PrimirestWeeklyMenu>>)JsonConvert.DeserializeObject<List<PrimirestWeeklyMenu>>(menusJson)!);

    public Task<int[]> GetMenuIdsAsync(HttpClient adminSessionLoggedClient)
        => Task.FromResult(new[] {141248686, 144357172, 146494128, 148185209});
}