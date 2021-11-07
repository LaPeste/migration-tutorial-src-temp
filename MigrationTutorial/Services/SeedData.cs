using System;
using System.Linq;
using MigrationTutorial.Models.V2;
using Realms;

namespace MigrationTutorial.Services
{
    public static class SeedData
    {
        public static void Seed()
        {
            var realm = RealmService.GetRealm();

            if (realm.Config.SchemaVersion < 2)
            {
                Migrations.V1Utils.SeedData();
            }
            else if (realm.Config.SchemaVersion < 3)
            {
                Migrations.V2Utils.SeedData();
            }
            else if (realm.Config.SchemaVersion < 4)
            {
                Migrations.V3Utils.SeedData();
            }
        }
    }
}
