using System;
using System.IO;
using Realms;

namespace MigrationTutorial.Services
{
    public class RealmService
    {
        private static RealmConfiguration conf = new RealmConfiguration(GetPath());

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
