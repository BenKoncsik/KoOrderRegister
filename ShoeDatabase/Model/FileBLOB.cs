using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeDatabase.Model
{
    public class FileBLOB
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public long ID { get; set; }
        public FileBLOB() { }
        public FileBLOB(string name, byte[] data) {  Name = name; Data = data; }
    }
}
