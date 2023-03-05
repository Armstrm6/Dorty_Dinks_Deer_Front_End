using System;
using System.Collections.Generic;
using System.Drawing;

namespace DinksCodeChallenge.Models
{
    public partial class DinkData
    {
        public DinkData()
        {
            DinkPictures = new HashSet<DinkPictures>();
        }
        public int DeerId { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }
        public DateTime DeerDate { get; set; }

        

        public virtual ICollection<DinkPictures> DinkPictures { get; set; }
    }
}
