#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccontsBank.Models;
public class Transaction
{
    [Key]
    public int TransactionId { get; set; }

    public int Amount { get; set; }

    public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
    public DateTime Fecha_Actualizacion { get; set; } = DateTime.Now;

    public int UserId {get; set;}

    public User? Creador {get; set;}
}