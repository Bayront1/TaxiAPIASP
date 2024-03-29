using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? UserType { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Carname { get; set; }

    public string? Carnumber { get; set; }

    public string? Image { get; set; }

    public int? PassengerRideId { get; set; }

}
