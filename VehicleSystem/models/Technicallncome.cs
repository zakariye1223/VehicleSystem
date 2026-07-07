namespace VehicleSystem.models
{
    
        public class TechnicalIncome
        {
            public int Id { get; set; }
            public string VehicleType { get; set; }  // "A", "B", or "C"
            public DateTime Month { get; set; }      // kaydi taariikhda 1aad ee bisha, tusaale: 2026-07-01
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }


