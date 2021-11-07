using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V3
{
    public class Customer : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string? Name { get; set; }

        public string Location { get; set; }

        public Customer() { }
    }
}
