using System;
using System.Linq;
using MigrationTutorial.Models.V2;
using Realms;

namespace MigrationTutorial.Services
{
    public static class SeedData
    {
        public static void Seed(Realm realm)
        {
            if (realm.Config.SchemaVersion == 1)
            {
                Migrations.V1Utils.SeedData(realm);
            }
            else if (realm.Config.SchemaVersion > 1)
            {
                Migrations.V2Utils.SeedData(realm);
            }
        }
    }
}
