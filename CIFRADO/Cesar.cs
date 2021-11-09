using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CIFRADO
{
    public class Cesar
    {
        Dictionary<int, byte> DWithkey = new Dictionary<int, byte>();
        public void eliminar(string Ruta, string Ruta2)
        {
            File.Delete(Ruta);
            File.Delete(Ruta2);
        }

        public void eliminar2(string Ruta)
        {
            File.Delete(Ruta);
        }
        public void Cifrar(String Ruta1, String Ruta2,string llave)
        {
            DWithkey = new Dictionary<int, byte>();
            Bibliotecas(llave);
            FileStream ArchivoOriginal = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoOriginal);
            FileStream ArchivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                var buffer = leer.ReadBytes(100);
                byte[] escribir = new byte[buffer.Length];
                int count = 0;
                foreach (var y in buffer)
                {
                    int posicion = Convert.ToInt16(y);
                    escribir[count] = DWithkey[posicion];
                    count++;
                }
                ArchivoCifrado.Write(escribir);
                ArchivoCifrado.Flush();
            }
            ArchivoCifrado.Close();
            ArchivoOriginal.Close();
        }
        public void DeCifrar(String Ruta1, String Ruta2, string llave)
        {
            DWithkey = new Dictionary<int, byte>();
            Bibliotecas(llave);
            var TempD = new Dictionary<byte, int>();
            foreach(var k in DWithkey)
            {
                TempD.Add(k.Value, k.Key);
            }
            FileStream ArchivoCifrado = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoCifrado);
            FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            while (ArchivoCifrado.Position < ArchivoCifrado.Length)
            {
                var buffer = leer.ReadBytes(100);
                byte[] escribir = new byte[buffer.Length];
                int count = 0;
                foreach (var y in buffer)
                {
                    int caracterreal = TempD[y];
                    escribir[count] = Convert.ToByte(caracterreal);
                    count++;
                }
                ArchivoDescifrado.Write(escribir);
                ArchivoDescifrado.Flush();
            }
            ArchivoDescifrado.Close();
            ArchivoCifrado.Close();
        }
        public void Bibliotecas(string llave)
        {
            Dictionary<byte, int> TempD = new Dictionary<byte, int>();
            foreach (var y in llave)
            {
                if (!TempD.ContainsKey(Convert.ToByte(y)))
                {
                    TempD.Add(Convert.ToByte(y), TempD.Count());
                }
            }
            int count = 0;
            while (TempD.Count() != 256)
            {
                if (!TempD.ContainsKey(Convert.ToByte(count)))
                {
                    TempD.Add(Convert.ToByte(count), TempD.Count);
                }
                count++;
            }
            foreach (var x in TempD)
            {
                DWithkey.Add(x.Value, x.Key);
            }
        }
    }
}
