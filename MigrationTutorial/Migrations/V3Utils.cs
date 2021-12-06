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
                Logger.LogInfo("Seed data: add Workshop department");

                realm.Add(new Department()
                {
                    Name = "Workshop",
                    Head = headWorkshop
                });

                Logger.LogInfo("Seed data: add MachineryAndTools");

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

        public static void DoMigrate(Migration migration, ulong oldSchemaVersion)
        {
            Realm newRealm = migration.NewRealm;
            Realm oldRealm = migration.OldRealm;
            var config = new RealmConfiguration("temp.realm")
            {
                SchemaVersion = oldSchemaVersion,
                IsDynamic = true
            };

            if (oldSchemaVersion < 2)
            {
                Realm.DeleteRealm(config);
                newRealm.WriteCopy(config);
                oldRealm = Realm.GetInstance(config);
            }


            Logger.LogInfo("In migration: convert GlueHolder from a Consumable to a MachineryAndTool");
            ConvertConsumableToTool(newRealm, oldRealm, "GlueHolder");

            Logger.LogInfo("In migration: convert Brush from a Consumable to a MachineryAndTool");
            ConvertConsumableToTool(newRealm, oldRealm, "Brush");

            if (oldSchemaVersion < 2)
            {
                oldRealm.Dispose();
                Realm.DeleteRealm(config);
            }
        }

        private static void ConvertConsumableToTool(Realm newRealm, Realm oldRealm, string consumableType)
        {
            // it's assumed that there's always 1 and 1 only type of Consumable
            var oldConsumable = ((IQueryable<RealmObject>)oldRealm.DynamicApi.All("Consumable")).Filter("_Type == $0", consumableType).FirstOrDefault();
            if (oldConsumable == null)
            {
                Logger.LogWarning($"No consumable was found with type {consumableType}. Nothing to convert.");
                return;
            }

            Supplier consumableSupplier = null;
            string consumableBrand = string.Empty;

            var supplierId = oldConsumable.DynamicApi.Get<RealmObject>("Supplier")?.DynamicApi.Get<ObjectId>("Id");

            // if supplier has not been set yet
            if (supplierId != null)
            {
                consumableSupplier = newRealm.All<Supplier>().Filter("Id == $0", supplierId).FirstOrDefault();
                consumableBrand = oldConsumable.DynamicApi.Get<string>("Brand");
            }

            newRealm.Add(new MachineryAndTool()
            {
                Type = Type.ManufacturingTool,
                Status = OperationalStatus.Functioning,
                AssignedMaintaner = null,
                ToolName = oldConsumable.DynamicApi.Get<string>("_Type").ToString(),
                Supplier = consumableSupplier,
                Brand = consumableBrand
            });

            var newConsumables = newRealm.All<Consumable>();

            // ProductId could be empty because of human error
            var consumableProductId = oldConsumable.DynamicApi.Get<string>("ProductId");
            if (consumableProductId != string.Empty)
            {
                var consumableToRemove = newConsumables.Where(x => x.ProductId == consumableProductId).First();
                newRealm.Remove(consumableToRemove);
            }
        }
    }
}

#endif