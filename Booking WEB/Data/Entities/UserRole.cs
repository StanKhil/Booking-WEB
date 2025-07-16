using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class UserRole
    {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        [JsonIgnore]
        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}
