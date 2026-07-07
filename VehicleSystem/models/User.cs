namespace VehicleSystem.models
{
    
        public class User
        {
            public int? Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }       // plain password (only used on input, never stored as-is)
            public string? PasswordHash { get; set; }    // hashed password (stored in DB)
            public DateTime? CreatedAt { get; set; }
        }
    }


