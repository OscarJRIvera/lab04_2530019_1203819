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
        public CIFRADO.Cesar Cesar;
        public Dictionary<string, string> Nombres;
        private Singleton()
        {
            Cesar = new CIFRADO.Cesar();
            Nombres = new Dictionary<string, string>();
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
