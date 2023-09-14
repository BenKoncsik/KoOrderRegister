using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KoOrderRegister.Model
{
    public class Setting
    {
        public Setting() { }
        public Setting(string name) 
        {
            this.Name = name;
        }
        public Setting(int id, string name, string value) 
        {
            this.Id = id;
            this.Name = name;
            this.Value = value;
        }
        public Setting(string name, string value) 
        {
            this.Name = name;
            this.Value = value;
        }
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
