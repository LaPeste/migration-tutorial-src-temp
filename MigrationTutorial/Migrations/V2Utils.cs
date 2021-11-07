﻿using System;
using System.Collections.Generic;
using System.Linq;
using MigrationTutorial.Models.V2;
using MigrationTutorial.Services;
using MigrationTutorial.Utils;
using Realms;

namespace MigrationTutorial.Migrations
{
    public static class V2Utils
    {
        public static void SeedData()
        {
            var realm = RealmService.GetRealm();

            if (realm.All<Customer>().Count() > 0)
            {
                Logger.LogWarning("The database was already seeded with V2");
                return;
            }

            var employees = realm.All<Employee>();
            var headDep1 = employees.Where(e => e.Fullname == "Mario Rossi").FirstOrDefault();
            var headDep2 = employees.Where(e => e.Fullname == "Federica Bianchi").FirstOrDefault();

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

            var suppliers = realm.All<Supplier>();
            var manufactDep = realm.All<Department>().Where(d => d.Name == "Manufacturing").First();
            var consumables = realm.All<Consumable>();

            realm.Write(() =>
            {
                for (var i = 0; i < employees.Count(); i++)
                {
                    employees.ElementAt(i).Department = manufactDep;
                }

                for (var i = 0; i < consumables.Count(); i++)
                {
                    // it is expected to have a supplier for everything
                    var typeToSearch = consumables.ElementAt(i).Type;
                    var matchingSupplier = suppliers.Filter("ANY _SuppliedTypes = $0", typeToSearch.ToString()).First();
                    consumables.ElementAt(i).Supplier = matchingSupplier;
                }
            });
        }

        public static void DoMigrate(Migration migration)
        {
            var newEmployees = migration.NewRealm.All<Employee>();
            var oldEmployees = migration.OldRealm.DynamicApi.All("Employee");

            for (var i = 0; i < newEmployees.Count(); i++)
            {
                var newEmployee = newEmployees.ElementAt(i);
                var oldEmployee = oldEmployees.ElementAt(i);
                newEmployee.Gender = string.Equals(oldEmployee.Gender, "female", StringComparison.OrdinalIgnoreCase) ?
                    Gender.Female : Gender.Male;
            }

            var newConsumables = migration.NewRealm.All<Consumable>();
            var oldConsumables = migration.OldRealm.DynamicApi.All("Consumable");
            var distinctConsumableId = new HashSet<string>();
            var consumableToDelete = new List<Consumable>();
            for (var i = 0; i < newConsumables.Count(); i++)
            {
                var currConsumable = newConsumables.ElementAt(i);

                // remove duplicates since ProductId is the new PrimaryKey
                if (distinctConsumableId.Contains(currConsumable.ProductId))
                {
                    consumableToDelete.Add(currConsumable);
                }
                else
                {
                    distinctConsumableId.Add(currConsumable.ProductId);
                }
            }

            for (var i = 0; i < newConsumables.Count(); i++)
            {
                consumableToDelete.ForEach(x => migration.NewRealm.Remove(x));
                newConsumables.ElementAt(i).LastPurchasedPrice = oldConsumables.ElementAt(i).Price;
            }
        }
    }
}
 