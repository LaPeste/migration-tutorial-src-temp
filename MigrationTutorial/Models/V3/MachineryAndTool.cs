using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models.V3
{
    public class MachineryAndTool : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        private string _Type { get; set; }

        public Type Type
        {
            get => Enum.Parse<Type>(_Type);
            set => _Type = value.ToString();
        }

        private string _Status { get; set; }

        public OperationalStatus Status
        {
            get => Enum.Parse<OperationalStatus>(_Status);
            set => _Status = value.ToString();
        }

        public Employee? AssignedMaintaner { get; set; }

        public string Brand { get; set; }

        public string ToolName { get; set; }

        public Supplier Supplier { get; set; }

        public MachineryAndTool() { }
    }

    public enum OperationalStatus
    {
        Malfunctioning = 1,
        Functioning,
        UnderReparation,
        IssueReported,
        IssueInTriage,
    }

    public enum Type
    {
        ManufacturingMachine = 1,
        ManufacturingTool,
        PrototypingMachine,
        PrototypingTool
    }
}
