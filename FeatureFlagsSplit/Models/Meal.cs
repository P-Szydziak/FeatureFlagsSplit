using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeatureFlagsSplit
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "date")]
        public DateTime ExpireTimeDate { get; set; }
        public int Weight { get; set; }
        public int NumberOfCaloriesIn100G { get; set; }
    }
}