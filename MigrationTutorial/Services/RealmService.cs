using System;
using System.IO;
using Realms;
using MigrationTutorial.Migrations;
using MigrationTutorial.Utils;
using Realms.Schema;

namespace MigrationTutorial.Services
{
    public class RealmService
    {
        private static ulong _schemaVersion = 0;

        private static RealmConfiguration _realmConfiguration;

        public static Realm GetRealm() => Realm.GetInstance(_realmConfiguration);

        public static void Init(ulong schemaVersion)
        {
            if (_schemaVersion == 0)
            {
                var realmPath = GetPath();
                _schemaVersion = schemaVersion;

                if (File.Exists(realmPath) && schemaVersion == 1)
                {
                    try
                    {
                        Logger.LogDebug($"Since the realm already exists and the supplied schema version is 1, it's assumed you want to start from scratch.\n Deleting {realmPath}");
                        File.Delete(realmPath);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning($"It was not possible to delete the local realm at path {realmPath} because of an exception\n{e}");
                    }
                }

                RealmSchema schema = null;
                if (_schemaVersion < 2)
                {
                    schema = new[] { typeof(Models.V1.Consumable), typeof(Models.V1.Employee) };
                }
                else if (_schemaVersion < 3)
                {
                    schema = new[] { typeof(Models.V2.Consumable), typeof(Models.V2.Employee), typeof(Models.V2.Customer), typeof(Models.V2.Department), typeof(Models.V2.Supplier) };
                }
                else if (_schemaVersion < 4)
                {
                    schema = new[] { typeof(Models.V3.Consumable), typeof(Models.V3.Employee), typeof(Models.V3.Customer), typeof(Models.V3.Department), typeof(Models.V3.Supplier), typeof(Models.V3.MachineryAndTool) };
                }

                _realmConfiguration = new RealmConfiguration(realmPath)
                {
                    // version 0 is the original schema version
                    SchemaVersion = _schemaVersion,

                    Schema = schema,

                    MigrationCallback = (migration, oldSchemaVersion) =>
                    {
                        Console.WriteLine("We're in the migration method");

                        if (oldSchemaVersion < 2)
                        {
                            V2Utils.DoMigrate(migration);
                        }
                        else if (oldSchemaVersion < 3)
                        {
                            V3Utils.DoMigrate(migration);
                        }
                    }
                };
            }
            else
            {
                Logger.LogWarning($"You can't set the schema version more than once! It's currently set to {_schemaVersion}.");
            }
        }

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
                Logger.LogDebug($"It was not possible to create the realm at the binary path. The default SDK path will be used, instead.\n{e}");
                return "";
            }
        }
    }
}
