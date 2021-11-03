using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V2
{
    public class Consumable : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        public ConsumableType Type
        {
            get
            {
                return (ConsumableType)_Type;
            }
            set
            {

                _Type = (int)value;
            }
        }

        [Required]
        private int? _Type { get; set; }

        public int Quantity { get; set; } = 0;

        [Required]
        public string UnitOfMeasure { get; set; }

        public Supplier Supplier { get; set; }

        public float LastPurchasedPrice { get; set; }

        public string Brand { get; set; }

        public Consumable() { }
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
