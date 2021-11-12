using System;
using System.Linq;
using MigrationTutorial.Services;
using MigrationTutorial.Models.V3;
using Realms;
using Type = MigrationTutorial.Models.V3.Type;
using MigrationTutorial.Utils;
using MongoDB.Bson;
using System.Dynamic;
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
                    Supplier glueSupplier = null;

                    // the field "Supplier.Id" will not exist if jumping from V1 to V3, skipping V2
                    try
                    {
                        var supplierId = (ObjectId)oldConsumable.Supplier.Id;
                        glueSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).FirstOrDefault();
                    }
                    catch (RuntimeBinderException)
                    {
                        Logger.LogDebug($"The property {nameof(Supplier.Id)} doesn't exist on the old realm. This should likely mean that a migration from schema V1 to schema V3 was performed, not passing through 2.\n A null supplier will be set for this object.");
                    }

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
                    Supplier brushSupplier = null;

                    // the field "Supplier.Id" will not exist if jumping from V1 to V3, skipping V2
                    try
                    {
                        var supplierId = (ObjectId)oldConsumable.Supplier.Id;
                        brushSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).FirstOrDefault();
                    }
                    catch (RuntimeBinderException)
                    {
                        Logger.LogDebug($"The property {nameof(Supplier.Id)} doesn't exist on the old realm. This should likely mean that a migration from schema V1 to schema V3 was performed, not passing through 2.\n A null supplier will be set for this object.");
                    }

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

            // the following Ids will be empty if jumping from V1 to V3, skipping V2
            if (oldBrushId != string.Empty)
            {
                var brushToRemove = newConsumables.Where(x => x.ProductId == oldBrushId).First();
                migration.NewRealm.Remove(brushToRemove);
            }
            if (oldBrushId != string.Empty)
            {
                var glueHolderToRemove = newConsumables.Where(x => x.ProductId == oldGlueHolderId).First();
                migration.NewRealm.Remove(glueHolderToRemove);
            }
        }
    }
}
