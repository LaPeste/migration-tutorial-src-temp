using System;
using System.Linq;
using MigrationTutorial.Models;
using MigrationTutorial.Services;
using Realms;

namespace MigrationTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            Realm realm = RealmService.GetRealm();
            SeedData.Seed(realm);

            var employees = realm.All<Employee>().ToList();
        }
    }
}
