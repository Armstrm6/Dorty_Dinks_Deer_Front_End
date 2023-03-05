using System;
using System.Collections.Generic;

namespace DinksCodeChallenge.Models
{
    public partial class DinkPictures
    {
        public int PictureId { get; set; }
        public byte[] Picture { get; set; }
        public int DeerId { get; set; }

        public virtual DinkData Deer { get; set; }
    }
}
