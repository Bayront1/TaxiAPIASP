using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class Passengerrequest
{
    public int RequestId { get; set; }

    public int? PassengerId { get; set; }

    public double? StartPointLat { get; set; }

    public double? StartPointLng { get; set; }

    public double? EndPointLat { get; set; }

    public double? EndPointLng { get; set; }

    public DateTime? RequestTime { get; set; }
    public string Status { get; set; }

}
