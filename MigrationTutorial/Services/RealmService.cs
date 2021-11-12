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
                _schemaVersion = schemaVersion;

                RealmSchema schema = null;

                if (_schemaVersion == 1)
                {
                    schema = new[] { typeof(Models.V1.Consumable), typeof(Models.V1.Employee) };
                }
                else if (_schemaVersion == 2)
                {
                    schema = new[] { typeof(Models.V2.Consumable), typeof(Models.V2.Employee), typeof(Models.V2.Customer), typeof(Models.V2.Department), typeof(Models.V2.Supplier) };
                }
                else if (_schemaVersion == 3)
                {
                    schema = new[] { typeof(Models.V3.Consumable), typeof(Models.V3.Employee), typeof(Models.V3.Customer), typeof(Models.V3.Department), typeof(Models.V3.Supplier), typeof(Models.V3.MachineryAndTool) };
                }

                _realmConfiguration = new RealmConfiguration("migrationTutorial.realm")
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

                        // TODO let it run, remove else to see if from 1 to 3 it's all fine
                        // TODO additionally update the readme to tell that from 1 to 3, skipping 2 is fine

                        /* In a real case scenario the following check would not exist. The problem is that by the time one writes a migration for a certain schema version, it's granted that the current schema already has all the fields.
                         * Unfortunately, this is not the case here since the following branch is a piece of code that would only be written at a future point in time.
                         * To say it in other words, `if (oldSchemaVersion < 3)` would never exist while the models have gone through only 1 migration, from 1 to 2.
                         * One would add V3 migration only when the models have gone through a second set of modifications.
                         */
                        if (_schemaVersion > 2)
                        {
                            if (oldSchemaVersion < 3)
                            {
                                V3Utils.DoMigrate(migration);
                            }
                        }
                    }
                };

                var realmPath = _realmConfiguration.DatabasePath;
                if (File.Exists(realmPath) && schemaVersion == 1)
                {
                    try
                    {
                        Logger.LogDebug($"Since the realm already exists and the supplied schema version is 1, it's assumed that you want to start from scratch.\n Deleting {realmPath}");
                        Realm.DeleteRealm(_realmConfiguration);
                        Logger.LogInfo($"Realm is going to be create at:\n{realmPath}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning($"It was not possible to delete the local realm at path {realmPath} because of an exception\n{e}");
                    }
                }
                else
                {
                    Logger.LogInfo($"the Realm is located at:\n{realmPath}");
                }
            }
            else
            {
                Logger.LogWarning($"You can't set the schema version more than once! It's currently set to {_schemaVersion}.");
            }
        }
    }
}
