using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeDatabase
{
    public class Custumer
    {
        public Custumer() { }
        public Custumer(string name) 
        {
            this.Name = name;
            this.Id = -1;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TAJNumber { get; set; }
    }
}
