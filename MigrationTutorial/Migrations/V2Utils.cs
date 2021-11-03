using System.Collections.Generic;
using System.Linq;
using MigrationTutorial.Models.V2;
using Realms;

namespace MigrationTutorial.Migrations
{
    public static class V2Utils
    {
        public static void SeedData(Realm realm)
        {
            var headDep1 = realm.All<Employee>().Where(e => e.Fullname == "Mario Rossi").FirstOrDefault();
            var headDep2 = realm.All<Employee>().Where(e => e.Fullname == "Federica Bianchi").FirstOrDefault();
            realm.Write(() =>
            {
                var suppliers = new List<Supplier>();

                var supplier = new Supplier() { Name = "RoseWood" };
                supplier.AddConsumableType(new ConsumableType[]
                {
                    ConsumableType.Brush,
                    ConsumableType.Glue,
                    ConsumableType.GlueHolder
                });
                suppliers.Add(supplier);

                supplier = new Supplier() { Name = "Catalina" };
                supplier.AddConsumableType(new ConsumableType[]
                {
                    ConsumableType.SandPaper,
                    ConsumableType.Brush
                });
                suppliers.Add(supplier);

                supplier = new Supplier() { Name = "Montgomery" };
                supplier.AddConsumableType(ConsumableType.MaterialSheet);
                suppliers.Add(supplier);

                realm.Add(suppliers);

                realm.Add(new Department[]{
                    new Department()
                    {
                        Name = "Manufacturing",
                        Head = headDep1
                    },
                    new Department()
                    {
                        Name = "Prototyping",
                        Head = headDep2
                    }
                });

                realm.Add(new Customer[]{
                    new Customer()
                    {
                        Name = "Nuke",
                        Location = "United States"
                    },
                    new Customer()
                    {
                        Name = "Gacci",
                        Location = "Italy"
                    }
                });
            });
        }

        public static void DoMigrate(Migration migration, ulong oldSchemaVersion)
        {
            // TODO check if to remove oldSchemaVersion completely
            if (oldSchemaVersion > 1)
            {
                var employees = migration.NewRealm.All<Employee>();

                var manufactDep = migration.NewRealm.All<Department>().Where(d => d.Name == "Manufacturing").First();

                for (var i = 0; i < employees.Count(); i++)
                {
                    var currEmployee = employees.ElementAt(i);

                    // everyone starts in the manufacturing dep unless manually set already
                    currEmployee.Department ??= manufactDep;
                }

                var suppliers = migration.NewRealm.All<Supplier>();
                var consumables = migration.NewRealm.All<Consumable>();

                for (var i = 0; i < consumables.Count(); i++)
                {
                    var currConsumable = consumables.ElementAt(i);
                    var matchingSupplier = suppliers.Where(s => s.SuppliedTypes.Contains(currConsumable.Type)).FirstOrDefault();
                    currConsumable.Supplier = matchingSupplier;
                }
            }
        }
    }
}
