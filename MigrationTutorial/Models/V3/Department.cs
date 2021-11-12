﻿using System.Linq;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V3
{
    public class Department : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string Name { get; set; }

        public Employee Head { get; set; }

        [Backlink(nameof(Employee.Department))]
        public IQueryable<Employee> Employees { get; }
    }
}
