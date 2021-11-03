using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V1
{
    public class Consumable : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        public ConsumableType Type
        {
            get
            {
                return (ConsumableType)TypeId;
            }
            set
            {

                TypeId = (int)value;
            }
        }

        [Required]
        public int? TypeId { get; set; }

        public int Quantity { get; set; } = 0;

        [Required]
        public string UnitOfMeasure {get; set;}

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
