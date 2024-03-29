using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public double? Rating1 { get; set; }

    public int? RatingSize { get; set; }

}
