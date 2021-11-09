using System;
using System.Collections.Generic;
using System.Text;

namespace CIFRADO
{
    public interface IRSA
    {
        void llaves(int p, int q,String RutaPrivate, String RutaPublic,String RutaZip);
        void CoD(String Ruta,String Ruta2,String llave);
        int Coprimo(int fiN);
        void eliminar(String Ruta, String Ruta2);
        void eliminar2(String Ruta);
        bool verificarprimo(int n);
    }
}
