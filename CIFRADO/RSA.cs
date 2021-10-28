using System;
using System.Collections.Generic;
using System.Text;

namespace CIFRADO
{
    public class RSA : IRSA
    {
        public void comprimir()
        {

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
                    if ((Convert.ToDouble(x) / Convert.ToDouble(f)) % 1 == 0 && (Convert.ToDouble(fiN) / Convert.ToDouble(f)) % 1 == 0)
                    {
                        comprobar = false;
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

        public void llaves(int p, int q)
        {
            int n = p * q;
            int fiN = (p - 1) * (q - 1);
            int coprimo = Coprimo(fiN);
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
                    pos1 = +fiN;
                }
                int pos2 = matriz[1, 0] - resta2;
                while (pos2 < 0)
                {
                    pos2 = +fiN;
                }
                matriz[0, 0] = matriz[0, 1];
                matriz[1, 0] = matriz[1, 1];
                matriz[0, 1] = pos1;
                matriz[1, 1] = pos2;
            }
            string privada = n + "," + matriz[1, 1];
            string publica = n + "," + coprimo;
        }

    }
}
