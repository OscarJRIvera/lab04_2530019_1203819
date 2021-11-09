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
using System.Text;
using System.IO.Compression;

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
        public IActionResult Cipher([FromForm] IFormFile file, [FromForm] string key, [FromRoute] string method)
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
                F.Cesar.eliminar2(Ruta);
                FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
                file.CopyTo(ArchivoOriginal);
                ArchivoOriginal.Close();
                if (method == "Cesar" || method == "César")
                {
                    Cifrado temp = new Cifrado();
                    F.NombreO = file.FileName;
                    F.NombreC= nombre + ".csr";
                    
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + nombre + ".csr");
                    F.Cesar.eliminar2(Ruta2);
                    F.Cesar.Cifrar(Ruta, Ruta2, key);
                    FileStream archivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
                    FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/Cesar");
                    return ArchivoCifrado2;
                }
                else if (method == "ZigZag")
                {
                    Cifrado temp = new Cifrado();
                    F.NombreO = file.FileName;
                    F.NombreC = nombre + ".zz";
                    string Ruta2 = Path.GetFullPath("ArchivosCifrados\\" + nombre + ".zz");
                    F.Cesar.eliminar2(Ruta2);
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
            string Tipo = F.NombreC.Substring(F.NombreC.Length - 3, 3);
            string Ruta = Path.GetFullPath("ArchivosCifrados\\" + F.NombreC);
            string Ruta2 = Path.GetFullPath("ArchivosDescifrados\\" + F.NombreO);
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
            else if (F.NombreC.Substring(F.NombreC.Length - 2, 2) == "zz")
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
                F.NombreC= nombre + ".sdes";
                F.NombreO = file.FileName;
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
                FileStreamResult ArchivoCifrado2 = new FileStreamResult(archivoCifrado, "text/SDES");
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
            string Ruta = Path.GetFullPath("ArchivosCifrados\\" + F.NombreC);
            string Ruta2 = Path.GetFullPath("ArchivosDescifrados\\" + F.NombreO);
            string Ruta3 = Path.GetFullPath("PERMUTACIONES.txt");
            FileStream ArchivoCifrado = new FileStream(Ruta, FileMode.OpenOrCreate);
            file.CopyTo(ArchivoCifrado);
            ArchivoCifrado.Close();
            F.Sdes.DesCifrar(Ruta, Ruta2, valor, Ruta3);
            FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            FileStreamResult ArchivoDescifrado2 = new FileStreamResult(ArchivoDescifrado, "text/SDES");
            return ArchivoDescifrado2;
        }
        [HttpGet("rsa/keys/{p}/{q}")]
        public IActionResult KeysRSA([FromRoute] string p, [FromRoute] string q)
        {
            try
            {
                int p1 = 0;
                int q1 = 0;
                try
                {
                    p1 = int.Parse(p);
                    q1 = int.Parse(q);
                }
                catch 
                {
                    string error = "Valores p y q deben ser valores primos differentes";
                    return BadRequest(error);
                }
                if ((F.RSA.verificarprimo(p1) == false || F.RSA.verificarprimo(q1) == false)||( p1==q1) )
                {
                    string error = "Valores p y q deben ser valores primos differentes";
                    return BadRequest(error);
                }
                if (p1 <= 2 || q1 <= 2)
                {
                    string error = "Valores p y q deben ser valores primos differentes mayor que 2";
                    return BadRequest(error);
                }
                string RutaPrivate = Path.GetFullPath("keys\\" + "private.key");
                string RutaPublic = Path.GetFullPath("keys\\" + "public.key");
                String RutaZip= Path.GetFullPath("keyzipactual\\" + "llaves" + ".zip");
                string verificar= F.RSA.llaves(p1, q1, RutaPrivate, RutaPublic,RutaZip);
                if (verificar == "Error1")
                {
                    string error = "Valores p y q no funciona, el valor de la multiplicacion de p y q debe ser como minimo 255";
                    return BadRequest(error);
                }
                else if (verificar == "Error")
                {
                    string error = " Valores p y q no funciona, ingrese otro llaves";
                    return BadRequest(error);
                }
                FileStream ArchivoZip = new FileStream(RutaZip, FileMode.Open);
                return File(ArchivoZip, "application/zip", "Keys.zip"); ;
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
        [HttpPost("rsa/{nombre}")]
        public IActionResult CorDRSA([FromForm] IFormFile file, [FromForm] IFormFile llave, [FromRoute] string nombre)
        {
            try
            {
                string nombrenuevo = "";
                if (file == null)
                    return BadRequest();
                else
                {
                    nombrenuevo = nombre + ".txt";
                }
                string Ruta = Path.GetFullPath("ArchivosRSATemp\\" + "Or" + file.FileName);
                string Ruta2 = Path.GetFullPath("ArchivosRSATemp\\" + nombrenuevo);
                F.RSA.eliminar(Ruta, Ruta2);
                var reader = new StreamReader(llave.OpenReadStream());
                string result = reader.ReadLine();
                FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
                file.CopyTo(ArchivoOriginal);
                ArchivoOriginal.Close();
                F.RSA.Cifrar(Ruta, Ruta2, result);
                F.RSA.eliminar2(Ruta);
                FileStream ArchivoCoD = new FileStream(Ruta2, FileMode.Open);
                return File(ArchivoCoD, "text/txt", nombrenuevo);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }


    }
}
