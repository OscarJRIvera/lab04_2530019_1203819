using System;
using System.Collections.Generic;
using System.Text;

namespace CIFRADO
{
    public interface IRSA
    {
        void llaves(int p, int q);
        void comprimir();
        int Coprimo(int fiN);
    }
}
