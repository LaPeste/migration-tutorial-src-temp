using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V1
{
    public class Employee : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string Fullname { get; set; }

        [Required]
        public int? Age { get; set; }

        public string Gender { get; set; }
    }
}
