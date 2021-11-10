﻿using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V1
{
    public class Consumable : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        public string ProductId { get; set; }

        public ConsumableType Type
        {
            get => Enum.Parse<ConsumableType>(_Type);
            set => _Type = value.ToString();
        }

        public int Quantity { get; set; } = 0;

        [Required]
        public string UnitOfMeasure {get; set;}

        public float Price { get; set; }

        [Required]
        private string? _Type { get; set; } = string.Empty;

        public Consumable(string productId = "")
        {
            ProductId = productId;
        }

        private Consumable() { }
    }


    public enum ConsumableType
    {
        Glue = 1,
        SandPaper,
        Brush,
        GlueHolder,
        MaterialSheet
    }
}
