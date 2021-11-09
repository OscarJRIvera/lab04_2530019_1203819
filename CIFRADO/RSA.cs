using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Numerics;

namespace CIFRADO
{
    public class RSA : ICIFRADO
    {
        public List<int> ValoresM = new List<int>();
        public void Cifrar(String Ruta, String Ruta2, String ValorL)
        {
            FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
            using var leer = new BinaryReader(ArchivoOriginal);
            string[] split = ValorL.Split(",");
            FileStream ArchivoCoD = new FileStream(Ruta2, FileMode.OpenOrCreate);
            bool verificar2 = false;
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                var buffer1 = leer.ReadBytes(2);
                if (buffer1[0] == 0 && buffer1[1] == 9)
                {
                    verificar2 = true;
                }
                ArchivoOriginal.Position = ArchivoOriginal.Length;
            }
            if (verificar2 == true)
            {
                ArchivoOriginal.Close(); ArchivoCoD.Close();
                Decifrar(Ruta, Ruta2, ValorL);
            }
            else
            {
                ArchivoOriginal.Position = 0;
                List<byte> temp = new List<byte>();
                temp.Add(0); temp.Add(9);
                ArchivoCoD.Write(temp.ToArray());
                ArchivoCoD.Flush();
                while (ArchivoOriginal.Position < ArchivoOriginal.Length)
                {
                    var buffer = leer.ReadBytes(200);
                    List<byte> escribir = new List<byte>();
                    foreach (var k in buffer)
                    {
                        int doe = Convert.ToInt32(split[1]);
                        int doe2 = Convert.ToInt32(split[0]);
                        BigInteger valorelevado = BigInteger.ModPow(k, (BigInteger)doe, (BigInteger)doe2);
                        byte[] arreglo = BitConverter.GetBytes((long)valorelevado);
                        foreach (var l in arreglo)
                        {
                            escribir.Add(l);
                        }
                    }
                    ArchivoCoD.Write(escribir.ToArray());
                    ArchivoCoD.Flush();
                }
                
            }
            ArchivoCoD.Close();
            ArchivoOriginal.Close();
        }
        public void Decifrar(String Ruta, String Ruta2, String ValorL)
        {
            FileStream ArchivoOriginal = new FileStream(Ruta, FileMode.OpenOrCreate);
            using var leer = new BinaryReader(ArchivoOriginal);
            string[] split = ValorL.Split(",");
            FileStream ArchivoCoD = new FileStream(Ruta2, FileMode.OpenOrCreate);

            ArchivoOriginal.Position = 2;
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                var buffer = leer.ReadBytes(800);
                List<byte> escribir = new List<byte>();
                List<byte> ochobit = new List<byte>();
                foreach (var k in buffer)
                {
                    int doe = Convert.ToInt32(split[1]);
                    int doe2 = Convert.ToInt32(split[0]);
                    ochobit.Add(k);
                    if (ochobit.ToArray().Length == 8) 
                    {
                        int bit = BitConverter.ToInt32(ochobit.ToArray());
                        BigInteger valorelevado = BigInteger.ModPow(bit, (BigInteger)doe, (BigInteger)doe2);
                        byte escribirbyte;
                        if ((int)valorelevado > 255)
                        {
                            escribirbyte = 0;
                        }
                        else
                        {
                            escribirbyte = Convert.ToByte((long)valorelevado);
                        }
                        ochobit = new List<byte>();
                        escribir.Add(escribirbyte);
                    }

                }
                ArchivoCoD.Write(escribir.ToArray());
                ArchivoCoD.Flush();
            }
            ArchivoOriginal.Close(); ArchivoCoD.Close();

        }
        public int Coprimo(int fiN)
        {
            bool comprobar = false;
            int Coprimo = 0;
            for (int x = 2; x < fiN; x++)
            {
                comprobar = true;
                for (int f = 2; f <= x; f++)
                {
                    if ((Convert.ToDouble(x) / Convert.ToDouble(f)) % 1 == 0)
                    {
                        if ((Convert.ToDouble(fiN) / Convert.ToDouble(f)) % 1 == 0)
                        {
                            comprobar = false;
                        }

                    }
                }
                Coprimo = x;
                if (comprobar == true)
                {
                    return Coprimo;
                }
            }
            return Coprimo;
        }



        public void eliminar(string Ruta, string Ruta2)
        {
            File.Delete(Ruta);
            File.Delete(Ruta2);
        }

        public void eliminar2(string Ruta)
        {
            File.Delete(Ruta);
        }

        public string llaves(int p, int q,String RutaPrivate, String RutaPublic,String RutaZip)
        {
            int n = p * q;
            if (n < 255)
            {
                return"Error1";
            }
            int fiN = (p - 1) * (q - 1);
            int coprimo = Coprimo(fiN);
            if (coprimo == 0)
            {
                return "Error";
            }
            int[,] matriz = new int[2, 2];
            matriz[0, 0] = fiN; matriz[1, 0] = fiN;
            matriz[0, 1] = coprimo; matriz[1, 1] = 1;
            while (matriz[0, 1] != 1)
            {
                int valordiv = Convert.ToInt32(Math.Truncate(Convert.ToDouble(matriz[0, 0]) / Convert.ToDouble(matriz[0, 1])));
                int resta1 = valordiv * matriz[0, 1];
                int resta2 = valordiv * matriz[1, 1];
                int pos1 = matriz[0, 0] - resta1;
                while (pos1 < 0)
                {
                    pos1 = pos1 + fiN;
                }
                int pos2 = matriz[1, 0] - resta2;
                while (pos2 < 0)
                {
                    pos2 = pos2 + fiN;
                }
                matriz[0, 0] = matriz[0, 1];
                matriz[1, 0] = matriz[1, 1];
                matriz[0, 1] = pos1;
                matriz[1, 1] = pos2;
            }
            string privada = n + "," + matriz[1, 1];
            string publica = n + "," + coprimo;
            if (privada == publica)
            {
                return "Error";
            }
            else
            {
                ValoresM = new List<int>();
                File.WriteAllText(RutaPrivate, privada);
                File.WriteAllText(RutaPublic, publica);
                if (File.Exists(RutaZip))
                {
                    File.Delete(RutaZip);
                }
                using (var archive = ZipFile.Open(RutaZip, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(RutaPrivate, Path.GetFileName(RutaPrivate));
                    archive.CreateEntryFromFile(RutaPublic, Path.GetFileName(RutaPublic));
                }
                return "Exito";
            }
        }

        public bool verificarprimo(int n)
        {
            for (int i =2; i <n; i++)
            {
                if (n % i==0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
