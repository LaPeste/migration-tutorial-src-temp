﻿using System;
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

        private ISet<string> _SuppliedTypes { get; }

        [Ignored]
        public ISet<ConsumableType> SuppliedTypes { get; } = new HashSet<ConsumableType>();

        public void AddConsumableType(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                _SuppliedTypes.Add(consumable.ToString());
                SuppliedTypes.Add(consumable);
            }
        }

        public void AddConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Add(consumable.ToString());
            SuppliedTypes.Add(consumable);
        }

        public void RemoveConsumableType(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                _SuppliedTypes.Remove(consumable.ToString());
                SuppliedTypes.Remove(consumable);
            }
        }

        public void RemoveConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Remove(consumable.ToString());
            SuppliedTypes.Remove(consumable);
        }
    }
}
