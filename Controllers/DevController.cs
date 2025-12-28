// USO APENAS PARA DESENVOLVIMENTO / DEBUG

//using AutoHubProjeto.Models;
//using AutoHubProjeto.Helpers;
//using Microsoft.AspNetCore.Mvc;

//public class DevController : Controller
//{
//    private readonly ApplicationDbContext _db;

//    public DevController(ApplicationDbContext db)
//    {
//        _db = db;
//    }

//    [HttpGet]
//    public IActionResult ResetPasswordAutoHub()
//    {
//        var user = _db.Utilizadors.FirstOrDefault(u => u.Email == "vendas@autohub.pt");

//        if (user == null)
//            return Content("Utilizador não encontrado.");

//        user.PasswordHash = PasswordHelper.HashPassword("AutoHub@123");
//        _db.SaveChanges();

//        return Content("Password redefinida com sucesso.");
//    }
//}
