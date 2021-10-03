using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace CIFRADO
{
    public class ZigZag
    {
        public void Cifrar(String Ruta1, String Ruta2, string llave)
        {
            List<byte> temp1;
            int tamaño = 0;
            Dictionary<int, List<byte>> Olas = new Dictionary<int, List<byte>>();
            int conta = 1;
            while (conta <= Convert.ToInt32(llave))
            {
                List<byte> temp = new List<byte>();
                Olas.Add(conta, temp);
                conta++;
            }

            FileStream ArchivoOriginal = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoOriginal);
            FileStream ArchivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            conta = 1;
            bool comprobar = false;
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                var buffer = leer.ReadBytes(100);
                foreach (var y in buffer)
                {
                    if (comprobar == true)
                    {
                        var temp = Olas[conta];
                        temp.Add(y);
                        conta--;
                        if (conta < 1)
                        {
                            comprobar = false;
                            conta = 2;
                        }
                    }
                    else
                    {
                        var temp = Olas[conta];
                        temp.Add(y);
                        conta++;
                        if (conta > Olas.Count())
                        {
                            comprobar = true;
                            conta = Olas.Count() - 1;
                        }
                    }
                }
            }
            byte prueba = Convert.ToByte('$');
            for (int i = 1; i <= Olas.Count(); i++)
            {
                List<byte> temp2 = Olas[i];
                if (i == 1)
                {
                    temp1 = Olas[i];
                    tamaño = temp1.Count();
                }
                else if (i == Olas.Count())
                {
                    while (temp2.Count() != tamaño)
                    {
                        temp2.Add(Convert.ToByte('$'));
                    }
                }
                else
                {
                    while (temp2.Count() != tamaño * 2)
                    {
                        temp2.Add(Convert.ToByte('$'));
                    }
                }
            }
            byte[] Escribir = new byte[100];
            int contador = 0;
            for (int f = 1; f <= Olas.Count(); f++)
            {
                foreach (var k in Olas[f])
                {

                    Escribir[contador] = k;
                    contador++;
                    if (contador == 100)
                    {
                        ArchivoCifrado.Write(Escribir);
                        ArchivoCifrado.Flush();
                        Escribir = new byte[100];
                        contador = 0;
                    }
                }
            }
            int tam = 0;
            foreach (var o in Escribir)
            {
                if (o != 0)
                {
                    tam++;
                }
            }
            if (Escribir[0] != 0)
            {
                byte[] Escribir2 = new byte[tam];
                for (int i = 0; i < tam; i++)
                {
                    Escribir2[i] = Escribir[i];
                }
                ArchivoCifrado.Write(Escribir2);
                ArchivoCifrado.Flush();
            }
            ArchivoCifrado.Close();
            ArchivoOriginal.Close();
        }
        public void DeCifrar(string Ruta1, String Ruta2, string llave)
        {
            Dictionary<int, List<byte>> Olas = new Dictionary<int, List<byte>>();
            int conta = 1;
            while (conta <= Convert.ToInt32(llave))
            {
                List<byte> templist = new List<byte>();
                Olas.Add(conta, templist);
                conta++;
            }
            FileStream ArchivoCifrado = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoCifrado);
            FileStream ArchivoDescifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            ArchivoDescifrado.SetLength(0);
            ArchivoDescifrado.Flush();
            int tamaño = Convert.ToInt32(ArchivoCifrado.Length) / ((2 * Convert.ToInt32(llave)) - 2);
            int temp = tamaño; int temp3 = tamaño;
            int temp2 = temp * 2;
            int lista = 1;
            while (ArchivoCifrado.Position < ArchivoCifrado.Length)
            {
                var buffer = leer.ReadBytes(100);
                foreach (var y in buffer)
                {
                    if (!(lista > Convert.ToInt32(llave)))
                    {
                        var templista = Olas[lista];
                        templista.Add(y);
                        if (temp != 0)
                        {
                            temp--;
                            if (temp == 0)
                            {
                                lista++;
                            }
                        }
                        else if (Olas.Count() != lista)
                        {
                            temp2--;
                            if (temp2 == 0)
                            {
                                lista++;
                                temp2 = tamaño * 2;
                            }
                        }
                        else if (temp3 != 0)
                        {
                            temp3--;
                            if (temp3 == 0)
                            {
                                lista++;
                            }
                        }
                    }

                }
            }
            conta = 0;
            while (conta < Olas[1].Count())
            {
                int verificar = 1;
                byte[] Escribir = new byte[(2 * Convert.ToInt32(llave)) - 2];
                List<byte> Templista2 = new List<byte>();
                foreach (var k in Olas)
                {
                    var templista1 = k.Value;
                    if (verificar == 1 || verificar == Olas.Count)
                    {
                        Escribir[verificar - 1] = templista1[conta];
                    }
                    else
                    {
                        Escribir[verificar - 1] = templista1[conta * 2];
                        Templista2.Add(templista1[(conta * 2) + 1]);
                    }
                    verificar++;
                }
                int segundaP = 0;
                foreach (var f in Templista2)
                {
                    Escribir[Escribir.Length - segundaP - 1] = f;
                    segundaP++;
                }
                if (conta == Olas[1].Count() - 1)
                {
                    int cuantosignos = 0;
                    foreach (var o in Escribir)
                    {
                        if (o == 36)
                        {
                            cuantosignos++;
                        }
                    }
                    var temparreglo = Escribir;
                    Escribir = new byte[Escribir.Length - cuantosignos];
                    for (int i = 0; i < Escribir.Length; i++)
                    {
                        Escribir[i] = temparreglo[i];
                    }
                }
                ArchivoDescifrado.Write(Escribir);
                ArchivoDescifrado.Flush();
                conta++;
            }
            ArchivoDescifrado.Close();
            ArchivoCifrado.Close();
        }
    }
}
