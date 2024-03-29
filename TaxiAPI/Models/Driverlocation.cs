using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class Driverlocation
{
    public int DriverId { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Status { get; set; } 
}
