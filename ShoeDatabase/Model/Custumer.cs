using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Model
{
    public class Custumer
    {
        public Custumer() { }
        public Custumer(string name) 
        {
            this.Name = name;
            this.Id = -1;
        }
        public Custumer(long id, string name, string address, string note, string tajNumber)
        {
            this.Name = name;
            this.Id = id;
            this.Address = address;
            this.TAJNumber = tajNumber;
            this.Note = note;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TAJNumber { get; set; }

        public string Note { get; set; }
    }
}
