
namespace MigrationTutorial.Services
{
    public static class SeedData
    {
        public static void Seed()
        {
            var realm = RealmService.GetRealm();

#if SCHEMA_VERSION_1
            Migrations.V1Utils.SeedData();
#endif

#if SCHEMA_VERSION_2
            Migrations.V2Utils.SeedData();
#endif

#if SCHEMA_VERSION_3
            Migrations.V3Utils.SeedData();
#endif
        }
    }
}