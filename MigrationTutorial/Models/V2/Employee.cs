using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V2
{
    public class Employee : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string Fullname { get; set; }

        [Required]
        public int? Age { get; set; }

        public Department Department { get; set; }

        public Gender Gender
        {
            get
            {
                return (Gender)_Gender;
            }
            set
            {
                _Gender = (int)value;
            }
        }

        private int _Gender { get; set; } = 0;

        public Employee() { }
    }

    public enum Gender
    {
        Male = 1,
        Female
    }
}
