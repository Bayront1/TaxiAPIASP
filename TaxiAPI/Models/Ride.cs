using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class Ride
{
    public int RideId { get; set; }

    public int? PassengerId { get; set; }

    public int? DriverId { get; set; }

    public double? StartPointLat { get; set; }

    public double? StartPointLng { get; set; }

    public double? EndPointLat { get; set; }

    public double? EndPointLng { get; set; }

}
