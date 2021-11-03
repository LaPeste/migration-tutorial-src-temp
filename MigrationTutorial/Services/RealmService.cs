using System;
using System.IO;
using Realms;
using MigrationTutorial.Migrations;

namespace MigrationTutorial.Services
{
    public class RealmService
    {
        private static RealmConfiguration conf = new RealmConfiguration(GetPath())
        {
            SchemaVersion = 1,

            MigrationCallback = (migration, oldSchemaVersion) =>
            {
                Console.WriteLine("We're in the migration method");

                if (oldSchemaVersion > 1)
                {
                    V2Utils.DoMigrate(migration, oldSchemaVersion);
                }
                /*
                 * TODO 3/11/2021
                 * Make first migration for
                 * 1- add department to employees
                 * 2- link consumales to suppliers
                */



                    //var oldPeople = migration.OldRealm.DynamicApi.All("Person");
                    //var newPeople = migration.NewRealm.All<Person>();

                    //// Migrate Person objects
                    //for (var i = 0; i < newPeople.Count(); i++)
                    //{
                    //    var oldPerson = oldPeople.ElementAt(i);
                    //    var newPerson = newPeople.ElementAt(i);

                    //    // Changes from version 1 to 2 (adding LastName) will occur automatically when Realm detects the change
                    //    // Migrate Person from version 2 to 3: replace FirstName and LastName with FullName
                    //    // LastName doesn't exist in version 1
                    //    if (oldSchemaVersion < 2)
                    //    {
                    //        newPerson.FullName = oldPerson.FirstName;
                    //    }
                    //    else if (oldSchemaVersion < 3)
                    //    {
                    //        newPerson.FullName = $"{oldPerson.FirstName} {oldPerson.LastName}";
                    //    }

                    //    // Migrate Person from version 3 to 4: replace Age with Birthday
                    //    if (oldSchemaVersion < 4)
                    //    {
                    //        var birthYear = DateTimeOffset.UtcNow.Year - oldPerson.Age;
                    //        newPerson.Birthday = new DateTimeOffset(birthYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
                    //    }
                    //}
            }
        };

        public static Realm GetRealm() => Realm.GetInstance(conf);

        private static string GetPath()
        {
            try
            {
                var binPath = AppDomain.CurrentDomain.BaseDirectory;
                var realmDir = Path.Combine(binPath, "RealmDB");

                Directory.CreateDirectory(Path.Combine(binPath, realmDir));
                var finalRealmPath = Path.Combine(binPath, realmDir, "migrationTutorial.realm");
                using (File.Create(Path.Combine(realmDir, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)) { };
                return finalRealmPath;
            }
            catch (Exception e)
            {
                Utils.Logger.LogDebug($"It was not possible to create the realm at the binary path. The default SDK path will be used, instead.\n{e}");
                return "";
            }
        }
    }
}
