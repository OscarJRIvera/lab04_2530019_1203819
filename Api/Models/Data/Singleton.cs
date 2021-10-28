using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIFRADO;
namespace Api.Models.Data
{
    public class Singleton
    {
        private readonly static Singleton _instance = new Singleton();
        public Cesar Cesar;
        public Dictionary<string, string> Nombres;
        public ZigZag Zigzag;
        public CIFRADO.SDES Sdes;
        public IRSA RSA;
        private Singleton()
        {
            RSA = new RSA();
            Cesar = new Cesar();
            Nombres = new Dictionary<string, string>();
            Zigzag = new ZigZag();
            Sdes = new CIFRADO.SDES();
        }
        public static Singleton Instance
        {

            get
            {
                return _instance;
            }
        }
    }
}
