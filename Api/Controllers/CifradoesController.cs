using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using System.IO;

namespace Api.Controllers
{
    [Route("Api")]
    [ApiController]
    public class CifradoesController : ControllerBase
    {
        private readonly Models.Data.Singleton F = Models.Data.Singleton.Instance;
        [HttpGet]
        public string Get()
        {
            string xd = "Lab04";

            return xd;
        }
        [HttpPost("Cypher/{tipo}/{name}")]
        public IActionResult Cypher([FromForm] IFormFile file, [FromForm] string key,[FromRoute] string tipo, [FromRoute] string name)
        {
            try
            {
                if (file == null)
                    return BadRequest();

                string Ruta = Path.GetFullPath("ArchivosOriginales\\" + name + file.FileName);
                FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
                file.CopyTo(ArchivoOriginal);
                ArchivoOriginal.Close();
                if (tipo == "Cesar")
                {
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + name + ".Ces");
                    F.Cesar.Cifrar(Ruta, Ruta2, key);
                    FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                    FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/Cesar");
                    Cifrado temp = new Cifrado();
                    temp.NombreOriginal = name + file.FileName;
                    temp.NombreCifrado = name + ".Ces";
                    F.Nombres.Add(temp.NombreCifrado, temp.NombreOriginal);
                    return ArchivoCifrado2;
                }
                else if(tipo=="ZigZag")
                {
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + name + ".Zig");
                    F.Cesar.Cifrar(Ruta, Ruta2, key);
                    FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                    FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/Cesar");
                    Cifrado temp = new Cifrado();
                    temp.NombreOriginal = name + file.FileName;
                    temp.NombreCifrado = name + ".Zig";
                    F.Nombres.Add(temp.NombreCifrado, temp.NombreOriginal);
                    return ArchivoCifrado2;
                }
                else
                {
                    string json = "Solo existe cifrado Cesar y ZigZag";
                    return BadRequest(json);
                }
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpPost("DeCypher")]
        public IActionResult DeCypher([FromForm] IFormFile file, [FromForm] string key)
        {
            if (file == null)
                return BadRequest();
            string Tipo = file.FileName.Substring(file.FileName.Length - 3, 3);
            string Ruta = Path.GetFullPath("ArchivosCifrados\\" + file.FileName);
            string nombre = F.Nombres[file.FileName];
            string Ruta2 = Path.GetFullPath("ArchivosDescifrados\\" + nombre);
            FileStream ArchivoCifrado = new FileStream(Ruta, FileMode.OpenOrCreate);
            file.CopyTo(ArchivoCifrado);
            ArchivoCifrado.Close();
            if (Tipo == "Ces")
            {
                F.Cesar.DeCifrar(Ruta, Ruta2, key);
                FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/Cesar");
                return ArchivoDescifrado2;
            }
            else if(Tipo == "Zig")
            {

            }
            else
            {
                string json = "el archivo no esta cifrado con Cesar o ZigZag";
                return BadRequest(json);
            }
            return null;
        }


    }
}
