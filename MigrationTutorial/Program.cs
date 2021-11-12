using System;
using System.Collections.Generic;
using MigrationTutorial.Services;
using MigrationTutorial.Utils;
using Realms;

namespace MigrationTutorial
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {

#region Parameter parsing
                // TODO think if to remove this in case I leave only 1 param
                var cmdParams = new Dictionary<string, string>();
                ulong schemaVersion = 0;
                if (args.Length < 2)
                {
                    Logger.LogError($"No enough parameters where passed.\n{Logger.GetHelpString()}");
                }

                var schemaVersionIndex = Array.FindIndex(args, x => x == "--schema_version");
                if (schemaVersionIndex != -1)
                {
                    cmdParams.Add("schema_version", args[schemaVersionIndex+1]);
                }
                else
                {
                    Logger.LogError($"Param schema_version not found");
                    return 1;
                }
#endregion

                schemaVersion = ulong.Parse(cmdParams["schema_version"]);
                if (schemaVersion > 3 || schemaVersion < 1)
                {
                    Logger.LogError($"Schema version {args[1]} is out of range. Only 1, 2 and 3 are currently supported.");
                    return 1;
                }
                RealmService.Init(schemaVersion);
                SeedData.Seed();
                return 0;
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception {e} was encountered.");
                return 1;
            }
        }
    }
}
