namespace VehicleSystem.models
{
   
        public class VehicleData
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string VehicleType { get; set; }   // "A", "B", or "C"
            public DateTime EntryDate { get; set; }   // Raatiq
            public decimal Amount { get; set; }       // Lacag
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }


