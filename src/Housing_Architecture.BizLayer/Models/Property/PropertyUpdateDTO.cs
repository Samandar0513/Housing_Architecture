using Housing_Architecture.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing_Architecture.BizLayer.Models.Property
{
    public class PropertyUpdateDTO
    {
        public int CategoryId { get; set; }
        public int DistrictId { get; set; }
        public int RegionId { get; set; }
        public PropertyType PropertyType { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal? TotalArea { get; set; }
        public int? Rooms { get; set; }
        public int? Floor { get; set; }
        public int? BuiltYear { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Amenity ID lar (Rasmlar alohida PropertyPhotoController orqali boshqariladi)
        public List<int>? AmenityIds { get; set; }
    }
}
