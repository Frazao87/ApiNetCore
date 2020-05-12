using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Models
{
    public class Pessoa
    {
        public Pessoa()
        {
            ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
    }
}
