using System;
using System.Linq;
using MigrationTutorial.Models.V1;
using Realms;

namespace MigrationTutorial.Migrations
{
    public static class V1Utils
    {
        public static void SeedData(Realm realm)
        {
            // since employees must be always there, if none, then the realm is just created
            if (realm.All<Employee>().ToList().Count == 0)
            {
                realm.Write(() =>
                {
                    realm.Add(new Employee[]{
                        new Employee()
                        {
                            Fullname = "Mario Rossi",
                            Age = 25,
                            Gender = Gender.Male
                        },
                        new Employee()
                        {
                            Fullname = "Federica Bianchi",
                            Age = 23,
                            Gender = Gender.Female
                        },
                        new Employee()
                        {
                            Fullname = "Luigi Verdi",
                            Age = 27,
                            Gender = Gender.Male
                        },
                        new Employee()
                        {
                            Fullname = "Giovanni Viola",
                            Age = 29,
                            Gender = Gender.Male
                        }
                        });

                    realm.Add(new Consumable[]
                    {
                        new Consumable()
                        {
                            Type = ConsumableType.Glue,
                            UnitOfMeasure = "Liters"
                        },
                        new Consumable()
                        {
                            Type = ConsumableType.Brush,
                            UnitOfMeasure = "Pieces"
                        },
                        new Consumable()
                        {
                            Type = ConsumableType.GlueHolder,
                            UnitOfMeasure = "Pieces"
                        },
                        new Consumable()
                        {
                            Type = ConsumableType.SandPaper,
                            UnitOfMeasure = "Strips"
                        },
                        new Consumable()
                        {
                            Type = ConsumableType.MaterialSheet,
                            UnitOfMeasure = "Pieces"
                        }
                    });
                });
            }
        }
    }
}
