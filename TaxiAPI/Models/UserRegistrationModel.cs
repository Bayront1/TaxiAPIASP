using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaxiAPI.Models;

public class UserRegistrationModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; }
}
