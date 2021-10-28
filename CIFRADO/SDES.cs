using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CIFRADO
{
    public class SDES
    {
        List<List<string>> s0 = new List<List<string>>();
        List<List<string>> s1 = new List<List<string>>();
        Dictionary<string, List<int>> permutaciones = new Dictionary<string, List<int>>();
        string clavebinario;
        string k1;
        string k2;
        public void Cifrar(String Ruta1, String Ruta2, int llave, String Rutapermutaciones)
        {
            Permutaciones(Rutapermutaciones);
            llaves(llave);
            matrices();
            FileStream ArchivoOriginal = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoOriginal);
            FileStream ArchivoCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            byte[] escribir = new byte[1000];
            int contador = 0;
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                var buffer = leer.ReadBytes(1);
                byte valor = Proceso1(buffer[0], k1, k2);
                escribir[contador] = valor;
                if (contador == 999)
                {
                    contador = -1;
                    ArchivoCifrado.Write(escribir);
                    ArchivoCifrado.Flush();
                    escribir = new byte[1000];
                }
                contador++;
            }
            var escribir2 = new byte[contador];
            for (int i = 0; i < contador; i++)
            {
                escribir2[i] = escribir[i];
            }
            ArchivoCifrado.Write(escribir2);
            ArchivoCifrado.Flush();
            ArchivoCifrado.Close();
            ArchivoOriginal.Close();
        }
        public void DesCifrar(String Ruta1, String Ruta2, int llave, String Rutapermutaciones)
        {
            Permutaciones(Rutapermutaciones);
            llaves(llave);
            matrices();
            FileStream ArchivoCifrado = new FileStream(Ruta1, FileMode.Open);
            using var leer = new BinaryReader(ArchivoCifrado);
            FileStream ArchivoDesCifrado = new FileStream(Ruta2, FileMode.OpenOrCreate);
            byte[] escribir = new byte[1000];
            int contador = 0;
            while (ArchivoCifrado.Position < ArchivoCifrado.Length)
            {
                var buffer = leer.ReadBytes(1);
                byte valor = Proceso1(buffer[0], k2, k1);
                escribir[contador] = valor;
                if (contador == 999)
                {
                    contador = -1;
                    ArchivoDesCifrado.Write(escribir);
                    ArchivoDesCifrado.Flush();
                    escribir = new byte[1000];
                }
                contador++;
            }
            var escribir2 = new byte[contador];
            for (int i = 0; i < contador; i++)
            {
                escribir2[i] = escribir[i];
            }
            ArchivoDesCifrado.Write(escribir2);
            ArchivoDesCifrado.Flush();
            ArchivoDesCifrado.Close();
            ArchivoCifrado.Close();
        }
        public byte Proceso1(byte Char, string Ka, string Kb)
        {
            string valor = Convert.ToString(Convert.ToInt32(Char), 2);
            while (valor.Length != 8)
            {
                valor = "0" + valor;
            }
            string temp1 = "";
            foreach (var k in permutaciones["IP"])
            {
                temp1 += valor.Substring(k - 1, 1);
            }
            string Firstfour = temp1.Substring(0, 4);
            string Lastfour = temp1.Substring(4, 4);

            //expandir y permutar
            string TempEP1 = "";
            foreach (var k in permutaciones["EP"])
            {
                TempEP1 += Lastfour.Substring(k - 1, 1);
            }
            //suma
            string sumaka = "";
            int posicion = 0;
            foreach (var f in TempEP1)
            {
                if (f == Ka[posicion])
                {
                    sumaka += "0";
                }
                else
                {
                    sumaka += "1";
                }
                posicion++;
            }

            //s0 s1
            string Firstfour2 = sumaka.Substring(0, 4);
            string Lastfour2 = sumaka.Substring(4, 4);
            int fila1 = Convert.ToInt32(Firstfour2.Substring(0, 1) + Firstfour2.Substring(3, 1), 2);
            int columna1 = Convert.ToInt32(Firstfour2.Substring(1, 1) + Firstfour2.Substring(2, 1), 2);
            int fila2 = Convert.ToInt32(Lastfour2.Substring(0, 1) + Lastfour2.Substring(3, 1), 2);
            int columna2 = Convert.ToInt32(Lastfour2.Substring(1, 1) + Lastfour2.Substring(2, 1), 2);
            string izuierda = s0[fila1][columna1];
            string derecha = s1[fila2][columna2];

            //p4
            string temp2 = izuierda + derecha;
            string p4 = "";
            foreach (var k in permutaciones["P4"])
            {
                p4 += temp2.Substring(k - 1, 1);
            }
            //suma p4 con los primeros 4
            string sumap4 = "";
            posicion = 0;
            foreach (var f in Firstfour)
            {
                if (f == p4[posicion])
                {
                    sumap4 += "0";
                }
                else
                {
                    sumap4 += "1";
                }
                posicion++;
            }

            string swap = Lastfour + sumap4;
            //parte 2
            return Proceso2(swap, Kb);
        }
        public byte Proceso2(string swap, string Kb)
        {
            // dividir swap
            string Firstfour = swap.Substring(0, 4);
            string Lastfour = swap.Substring(4, 4);
            //expandir y permutar
            string TempEP1 = "";
            foreach (var k in permutaciones["EP"])
            {
                TempEP1 += Lastfour.Substring(k - 1, 1);
            }
            //suma
            string sumakb = "";
            int posicion = 0;
            foreach (var f in TempEP1)
            {
                if (f == Kb[posicion])
                {
                    sumakb += "0";
                }
                else
                {
                    sumakb += "1";
                }
                posicion++;
            }

            //s0 s1
            string Firstfour2 = sumakb.Substring(0, 4);
            string Lastfour2 = sumakb.Substring(4, 4);
            int fila1 = Convert.ToInt32(Firstfour2.Substring(0, 1) + Firstfour2.Substring(3, 1), 2);
            int columna1 = Convert.ToInt32(Firstfour2.Substring(1, 1) + Firstfour2.Substring(2, 1), 2);
            int fila2 = Convert.ToInt32(Lastfour2.Substring(0, 1) + Lastfour2.Substring(3, 1), 2);
            int columna2 = Convert.ToInt32(Lastfour2.Substring(1, 1) + Lastfour2.Substring(2, 1), 2);
            string izuierda = s0[fila1][columna1];
            string derecha = s1[fila2][columna2];

            //p4
            string temp2 = izuierda + derecha;
            string p4 = "";
            foreach (var k in permutaciones["P4"])
            {
                p4 += temp2.Substring(k - 1, 1);
            }
            //suma p4 con los primeros 4
            string sumap4 = "";
            posicion = 0;
            foreach (var f in Firstfour)
            {
                if (f == p4[posicion])
                {
                    sumap4 += "0";
                }
                else
                {
                    sumap4 += "1";
                }
                posicion++;
            }

            string final = sumap4 + Lastfour;
            //ip-1
            string cifrado = "";
            foreach (var k in permutaciones["IP-1"])
            {
                cifrado += final.Substring(k - 1, 1);
            }
            byte escribir = Convert.ToByte(Convert.ToInt32(cifrado, 2));
            return escribir;
        }
        public void matrices()
        {
            string RutaS0 = Path.GetFullPath("S0.txt");
            string RutaS1 = Path.GetFullPath("S1.txt");
            string[] matrizs1 = System.IO.File.ReadAllLines(RutaS1);
            string[] matrizs0 = System.IO.File.ReadAllLines(RutaS0);
            s0 = new List<List<string>>();
            s1 = new List<List<string>>();
            for (int f = 0; f < 4; f++)
            {
                List<string> temp = new List<string>();
                s0.Add(temp);
                List<string> temp2 = new List<string>();
                s1.Add(temp2);
            }
            int fila = 0;
            foreach (var k in matrizs1)
            {
                string[] temp = k.Split(",");
                foreach (var f in temp)
                {
                    s1[fila].Add(f);
                }
                fila++;
            }
            fila = 0;
            foreach (var k in matrizs0)
            {
                string[] temp = k.Split(",");
                foreach (var f in temp)
                {
                    s0[fila].Add(f);
                }
                fila++;
            }
        }
        public void llaves(int llave)
        {
            string temp = "";
            string Firstfive = "";
            string Lastfive = "";
            string lsa1 = "";
            string lsb1 = "";
            k1 = "";
            k2 = "";
            clavebinario = Convert.ToString(llave, 2);
            while (clavebinario.Length != 10)
            {
                clavebinario = "0" + clavebinario;
            }
            foreach (var k in permutaciones["P10"])
            {
                temp += clavebinario.Substring(k - 1, 1);
            }
            //primera separacion/ls-1
            Firstfive = temp.Substring(0, 5);
            Lastfive = temp.Substring(5, 5);
            lsa1 = Firstfive.Substring(1, 4);
            lsa1 += Firstfive.Substring(0, 1);
            lsb1 = Lastfive.Substring(1, 4);
            lsb1 += Lastfive.Substring(0, 1);
            //p8 y primera llave
            temp = lsa1 + lsb1;
            foreach (var k in permutaciones["P8"])
            {
                k1 += temp.Substring(k - 1, 1);
            }
            //ls-2
            Firstfive = lsa1;
            Lastfive = lsb1;
            lsa1 = Firstfive.Substring(2, 3);
            lsa1 += Firstfive.Substring(0, 2);
            lsb1 = Lastfive.Substring(2, 3);
            lsb1 += Lastfive.Substring(0, 2);
            //p8 y segunda llave
            temp = lsa1 + lsb1;
            foreach (var k in permutaciones["P8"])
            {
                k2 += temp.Substring(k - 1, 1);
            }
        }
        public void Permutaciones(String RutaP)
        {
            permutaciones = new Dictionary<string, List<int>>();
            string[] perm = System.IO.File.ReadAllLines(RutaP);
            int contador = 0;
            foreach (var k in perm)
            {
                contador++;
                List<int> templista = new List<int>();
                string[] temp = k.Split(",");
                foreach (var y in temp)
                {
                    templista.Add(Convert.ToInt32(y));
                }
                if (contador == 1)
                    permutaciones.Add("P10", templista);
                if (contador == 2)
                    permutaciones.Add("P8", templista);
                if (contador == 3)
                    permutaciones.Add("P4", templista);
                if (contador == 4)
                    permutaciones.Add("EP", templista);
                if (contador == 5)
                    permutaciones.Add("IP", templista);
                if (contador == 6)
                    permutaciones.Add("IP-1", templista);

            }

        }
    }
}
