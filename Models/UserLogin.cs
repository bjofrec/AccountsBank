#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace AccontsBank.Models;
public class UserLogin
{
    [Required]    
    [EmailAddress]
    public string Email { get; set; }    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } 
}