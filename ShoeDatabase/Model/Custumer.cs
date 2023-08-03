using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeDatabase.Model
{
    public class Custumer
    {
        public Custumer() { }
        public Custumer(string name) 
        {
            this.Name = name;
            this.Id = -1;
        }
        public Custumer(int id, string name, string address, string tajNumber)
        {
            this.Name = name;
            this.Id = id;
            this.Address = address;
            this.TAJNumber = tajNumber;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TAJNumber { get; set; }
    }
}
