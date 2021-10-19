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
    [Route("api")]
    [ApiController]
    public class CifradoesController : ControllerBase
    {
        private readonly Models.Data.Singleton F = Models.Data.Singleton.Instance;
        [HttpGet]
        public string Get()
        {
            string xd = "Lab04 y lab05";

            return xd;
        }
        [HttpPost("cipher/{method}")]
<<<<<<< Updated upstream
        public IActionResult Cipher([FromForm] IFormFile file, [FromForm] string key,[FromRoute] string method)
=======
        public IActionResult Cipher([FromForm] IFormFile file, [FromForm] string key, [FromRoute] string method)
>>>>>>> Stashed changes
        {
            try
            {
                string nombre = "";
                if (file == null)
                    return BadRequest();
                else
                {
                    nombre = file.FileName;
                    nombre = nombre.Replace(".txt", "");
                }
                string Ruta = Path.GetFullPath("ArchivosOriginales\\" + file.FileName);
                FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
                file.CopyTo(ArchivoOriginal);
                ArchivoOriginal.Close();
                if (method == "Cesar" || method== "César")
                {
                    Cifrado temp = new Cifrado();
                    temp.NombreOriginal = file.FileName;
                    temp.NombreCifrado = nombre + ".csr";
                    try
                    {
                        F.Nombres.Add(temp.NombreCifrado, temp.NombreOriginal);
                    }
                    catch
                    {
                        string json = "El nombre del archivo que desea comprimir ya existe";
                        ArchivoOriginal.Close();
                        return BadRequest(json);
                    }
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + nombre + ".csr");
                    F.Cesar.Cifrar(Ruta, Ruta2, key);
                    FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                    FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/Cesar");
                    return ArchivoCifrado2;
                }
                else if(method == "ZigZag")
                {
                    Cifrado temp = new Cifrado();
                    temp.NombreOriginal = file.FileName;
                    temp.NombreCifrado = nombre + ".zz";
                    try
                    {
                        F.Nombres.Add(temp.NombreCifrado, temp.NombreOriginal);
                    }
                    catch
                    {
                        string json = "El nombre del archivo que desea comprimir ya existe";
                        ArchivoOriginal.Close();
                        return BadRequest(json);
                    }
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + nombre + ".zz");
                    F.Zigzag.Cifrar(Ruta, Ruta2, key);
                    FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                    FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/ZigZag");
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
        [HttpPost("decipher")]
        public IActionResult DeCipher([FromForm] IFormFile file, [FromForm] string key)
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
            if (Tipo == "csr")
            {
                F.Cesar.DeCifrar(Ruta, Ruta2, key);
                FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/Cesar");
                return ArchivoDescifrado2;
            }
            else if(file.FileName.Substring(file.FileName.Length - 2, 2) == "zz")
            {
                F.Zigzag.DeCifrar(Ruta, Ruta2, key);
                FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/ZigZag");
                return ArchivoDescifrado2;
            }
            else
            {
                string json = "el archivo no esta cifrado con Cesar o ZigZag";
                return BadRequest(json);
            }
            return null;
        }

        [HttpPost("sdes/cipher/{nombre}")]
        public IActionResult Ciphersdes([FromForm] IFormFile file, [FromForm] string key, [FromRoute] string nombre)
        {
            try
            {
                int valor = 0;
                if (file == null)
                    return BadRequest();
                try
                {
                    valor = Convert.ToInt32(key);
                    if (valor < 0 || valor > 1023)
                    {
                        string json = "Valor debe ser in entero positivo no mayor a 1023";
                        return BadRequest(json);
                    }
                }
                catch
                {
                    string json = "Valor de la llave debe de ser un int";
                    return BadRequest(json);
                }
                string Ruta = Path.GetFullPath("ArchivosOriginales\\" + file.FileName);
                FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
                file.CopyTo(ArchivoOriginal);
                ArchivoOriginal.Close();
                Cifrado temp = new Cifrado();
                temp.NombreOriginal = file.FileName;
                temp.NombreCifrado = nombre + ".sdes";
                try
                {
                    F.Nombres.Add(temp.NombreCifrado, temp.NombreOriginal);
                }
                catch
                {
                    string json = "El nombre del archivo que desea comprimir ya existe";
                    ArchivoOriginal.Close();
                    return BadRequest(json);
                }
                string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + nombre + ".sdes");
                string Ruta3 = Path.GetFullPath("PERMUTACIONES.txt");
                F.Sdes.Cifrar(Ruta, Ruta2, valor, Ruta3);
                FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
<<<<<<< Updated upstream
                FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/Cesar");
=======
                FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/SDES");
>>>>>>> Stashed changes
                return ArchivoCifrado2;
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpPost("sdes/decipher")]
        public IActionResult DeCiphersdes([FromForm] IFormFile file, [FromForm] string key)
        {
<<<<<<< Updated upstream
            if (file == null)
                return BadRequest();
            string Ruta = Path.GetFullPath("ArchivosCifrados\\" + file.FileName);
            string nombre = F.Nombres[file.FileName];
            string Ruta2 = Path.GetFullPath("ArchivosDescifrados\\" + nombre);
            FileStream ArchivoCifrado = new FileStream(Ruta, FileMode.OpenOrCreate);
            file.CopyTo(ArchivoCifrado);
            ArchivoCifrado.Close();
            FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/Cesar");
=======
            int valor = 0;
            if (file == null)
                return BadRequest();
            try
            {
                valor = Convert.ToInt32(key);
                if (valor < 0 || valor > 1023)
                {
                    string json = "Valor debe ser in entero positivo no mayor a 1023";
                    return BadRequest(json);
                }
            }
            catch
            {
                string json = "Valor de la llave debe de ser un int";
                return BadRequest(json);
            }
            string Ruta = Path.GetFullPath("ArchivosCifrados\\" + file.FileName);
            string nombre = F.Nombres[file.FileName];
            string Ruta2 = Path.GetFullPath("ArchivosDescifrados\\" + nombre);
            string Ruta3 = Path.GetFullPath("PERMUTACIONES.txt");
            FileStream ArchivoCifrado = new FileStream(Ruta, FileMode.OpenOrCreate);
            file.CopyTo(ArchivoCifrado);
            ArchivoCifrado.Close();
            F.Sdes.DesCifrar(Ruta, Ruta2, valor, Ruta3);
            FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/SDES");
>>>>>>> Stashed changes
            return ArchivoDescifrado2;
        }


    }
}
