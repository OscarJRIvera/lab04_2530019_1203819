using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CIFRADO
{
    public class SDES
    {
        Dictionary<string, List<int>> permutaciones = new Dictionary<string, List<int>>();
        public void Cifrar(String Ruta1, String Ruta2, int llave, String Rutapermutaciones)
        {
            Permutaciones(Rutapermutaciones);
        }
        public void llaves()
        {

        }
        public void Permutaciones(String RutaP)
        {
            string[] perm = System.IO.File.ReadAllLines(RutaP);
            int contador = 0;
            foreach(var k in perm)
            {
                contador++;
                List<int> templista = new List<int>();
                string[] temp = k.Split(",");
                foreach(var y in temp)
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
            int x = 0;
            
        }
    }
}
