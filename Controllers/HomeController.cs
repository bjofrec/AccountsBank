using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AccontsBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace AccontsBank.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;

        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost("user/create")]
    public IActionResult CreateUser(User user){
        if (ModelState.IsValid){
            PasswordHasher<User> Hasher = new PasswordHasher<User>();   
            user.Password = Hasher.HashPassword(user, user.Password); 
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            return RedirectToAction("Principal");
        }else{
            return View("Index");
        }
    }

    [HttpPost("/login")]
    public IActionResult Login(UserLogin userSubmission) 
    {

        if (!ModelState.IsValid) 
        {
            return View("Index"); 
        }
        
        User? user = _context.Users.FirstOrDefault(e => e.Email == userSubmission.Email);


        if (user == null)
        {
            ModelState.AddModelError("Email", "Invalid Email Address/Password");
            return View("Index");
        }
        PasswordHasher<UserLogin> hashbrowns = new PasswordHasher<UserLogin>();
        var result = hashbrowns.VerifyHashedPassword(userSubmission, user.Password, userSubmission.Password);
        if (result == 0)
        {
            ModelState.AddModelError("Email", "Invalid Email Address/Password");
        }
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetInt32("UUID", user.UserId);
        return View("Principal");

    }

    [HttpGet("/principal")]
    public IActionResult Principal()
    {
        IEnumerable<Transaction> transactions = _context.Transactions.Where(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        int totalAmount = transactions.Sum(s => s.Amount);
        ViewBag.Transactions = transactions;
        ViewBag.TotalAmount = totalAmount;
        return View("Principal");
    }

    public IActionResult Logout(){

        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpPost("update/balance")]
    public IActionResult UpdateBalance(Transaction newTransaction)
    {
        IEnumerable<Transaction> transactions = _context.Transactions.Where(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        int totalAmount = transactions.Sum(s => s.Amount);
        if (newTransaction.Amount < totalAmount)
        {
            ModelState.AddModelError("Amount", "No tienes saldo suficiente para realizar este retiro.");
            return RedirectToAction("Principal");

        }
        
        _context.Transactions.Add(newTransaction);
        _context.SaveChanges();

        return RedirectToAction("Principal");
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
