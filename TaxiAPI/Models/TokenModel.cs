using System;
using System.Collections.Generic;

namespace TaxiAPI.Models;

public partial class TokenModel
{

    public string Token { get; set; }

    public User User { get; set; }


}
