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

        public Gender Gender
        {
            get
            {
                return (Gender)GenderId;
            }
            set
            {
                GenderId = (int)value;
            }
        }

        public int GenderId { get; set; } = 0;

        public Employee() { }

    }

    public enum Gender
    {
        Male = 1,
        Female
    }
}
