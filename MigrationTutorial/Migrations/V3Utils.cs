using System;
using System.Linq;
using MigrationTutorial.Services;
using MigrationTutorial.Models.V3;
using Realms;
using Type = MigrationTutorial.Models.V3.Type;
using MigrationTutorial.Utils;
using MongoDB.Bson;
using System.Dynamic;

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

            var headWorkshop = realm.All<Employee>().Where(e => e.Fullname == "Giovanni Viola").FirstOrDefault();

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
            // TODO check if the filtering on dynamics is supposed to be supported or not
            //var oldGlueHolder = migration.OldRealm.DynamicApi.All("Consumable").Filter("_Type == \"GlueHolder\"").FirstOrDefault();
            //var oldBrush = migration.OldRealm.DynamicApi.All("Consumable").Filter("_Type == \"Brush\"").FirstOrDefault();

            string oldGlueHolderId = string.Empty;
            string oldBrushId = string.Empty;
            var oldConsumables = migration.OldRealm.DynamicApi.All("Consumable");

            for (var i = 0; i < oldConsumables.Count(); i++)
            {
                var oldConsumable = oldConsumables.ElementAt(i);
                if (oldConsumable._Type == "GlueHolder")
                {
                    var supplierId = (ObjectId)oldConsumable.Supplier.Id;
                    var glueSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).First();

                    migration.NewRealm.Add(new MachineryAndTool()
                    {
                        Type = Type.ManufacturingTool,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintaner = null,
                        ToolName = oldConsumable._Type,
                        Supplier = glueSupplier,
                        Brand = oldConsumable.Brand
                    });

                    oldGlueHolderId = oldConsumable.ProductId;
                }
                else if (oldConsumable._Type == "Brush")
                {
                    var supplierId = (ObjectId)oldConsumable.Supplier.Id;
                    var brushSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).First();

                    migration.NewRealm.Add(new MachineryAndTool()
                    {
                        Type = Type.ManufacturingTool,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintaner = null,
                        ToolName = oldConsumable._Type,
                        Supplier = brushSupplier,
                        Brand = oldConsumable.Brand
                    });

                    oldBrushId = oldConsumable.ProductId;
                }
            }

            var newConsumables = migration.NewRealm.All<Consumable>();
            var brushToRemove = newConsumables.Where(x => x.ProductId == oldBrushId).First();
            var glueHolderToRemove = newConsumables.Where(x => x.ProductId == oldGlueHolderId).First();
            migration.NewRealm.Remove(brushToRemove);
            migration.NewRealm.Remove(glueHolderToRemove);
        }
    }
}
