using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V2
{
    public class Supplier : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        public string Name { get; set; }

        //[Required]
        private ISet<int> _SuppliedTypes { get; }

        [Ignored]
        public ISet<ConsumableType> SuppliedTypes { get; } = new HashSet<ConsumableType>();

        public Supplier() { }

        public void AddConsumableType(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                _SuppliedTypes.Add((int)consumable);
                SuppliedTypes.Add(consumable);
            }
        }

        public void AddConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Add((int)consumable);
            SuppliedTypes.Add(consumable);
        }

        public void RemoveConsumableType(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                _SuppliedTypes.Remove((int)consumable);
                SuppliedTypes.Remove(consumable);
            }
        }

        public void RemoveConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Remove((int)consumable);
            SuppliedTypes.Remove(consumable);
        }
    }
}
