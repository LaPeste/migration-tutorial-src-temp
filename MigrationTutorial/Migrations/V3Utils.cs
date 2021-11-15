#if SCHEMA_VERSION_3

using System.Linq;
using MigrationTutorial.Services;
using MigrationTutorial.Models;
using Realms;
using Type = MigrationTutorial.Models.Type;
using MigrationTutorial.Utils;
using MongoDB.Bson;
using Microsoft.CSharp.RuntimeBinder;

namespace MigrationTutorial.Migrations
{
    public static class V3Utils
    {
        public static void SeedData()
        {
            var realm = RealmService.GetRealm();

            // where 2 is just the 2 items that have moved from Consumable to MachineryAndTool => brush and glue holder
            if (realm.All<MachineryAndTool>().Count() > 2)
            {
                Logger.LogWarning("The database was already seeded with V3");
                return;
            }

            var headWorkshop = realm.All<Employee>().Where(e => e.FullName == "Giovanni Viola").FirstOrDefault();

            realm.Write(() =>
            {
                realm.Add(new Department()
                {
                    Name = "Workshop",
                    Head = headWorkshop
                });

                realm.Add(new MachineryAndTool[]
                {
                    new MachineryAndTool()
                    {
                        Type = Type.ManufacturingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintaner = null,
                        ToolName = "Milling machine"
                    },
                    new MachineryAndTool()
                    {
                        Type = Type.ManufacturingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintaner = null,
                        ToolName = "Press"
                    },
                    new MachineryAndTool()
                    {
                        Type = Type.PrototypingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintaner = null,
                        ToolName = "Grinder"
                    }
                });
            });
        }

        public static void DoMigrate(Migration migration)
        {
            ConvertConsumableToTool(migration, "GlueHolder");
            ConvertConsumableToTool(migration, "Brush");
        }

        private static void ConvertConsumableToTool(Migration migration, string consumableType)
        {
            var oldConsumable = ((IQueryable<RealmObject>)migration.OldRealm.DynamicApi.All("Consumable")).Filter("_Type == $0", consumableType).FirstOrDefault();
            if (oldConsumable == null)
            {
                Logger.LogWarning($"No consumable was found with type {consumableType}. Nothing to convert.");
                return;
            }

            Supplier consumableSupplier = null;
            string consumableBrand = string.Empty;

            try
            {
                var supplierId = oldConsumable.DynamicApi.Get<RealmObject>("Supplier").DynamicApi.Get<ObjectId>("Id");
                consumableSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).FirstOrDefault();
                consumableBrand = oldConsumable.DynamicApi.Get<string>("Brand");
            }
            catch (System.MissingMemberException)
            {
                Logger.LogDebug($"Some properties don't exist on the old realm. This could likely mean that a migration from schema V1 to schema V3 was performed, not passing through 2.\n The operation will continue leaving such fields at their default value.");
            }

            migration.NewRealm.Add(new MachineryAndTool()
            {
                Type = Type.ManufacturingTool,
                Status = OperationalStatus.Functioning,
                AssignedMaintaner = null,
                ToolName = oldConsumable.DynamicApi.Get<string>("_Type").ToString(),
                Supplier = consumableSupplier,
                Brand = consumableBrand
            });

            var newConsumables = migration.NewRealm.All<Consumable>();

            // ProductId could be empty because of human error
            var consumableProductId = oldConsumable.DynamicApi.Get<string>("ProductId");
            if (consumableProductId != string.Empty)
            {
                var consumableToRemove = newConsumables.Where(x => x.ProductId == consumableProductId).First();
                migration.NewRealm.Remove(consumableToRemove);
            }
        }
    }
}

#endif