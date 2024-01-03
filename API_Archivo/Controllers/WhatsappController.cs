using API_Archivo.Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {

        [HttpGet]
        [Route("Generar_Token")]
        public IActionResult GenerarToken()
        {
            var token = Guid.NewGuid().ToString(); // Generar un token aleatorio utilizando Guid
            return new ContentResult
            {
                Content = token,
                ContentType = "text/plain",
                StatusCode = 200 // Código de estado OK (200)
            };
        }
    }
}