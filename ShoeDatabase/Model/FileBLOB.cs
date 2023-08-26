using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ShoeDatabase.Model
{
    public class FileBLOB
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public long ID { get; set; }
        public long ProductID { get; set; }
        public BitmapImage BitmapImage { get; set; }
        public FileBLOB() { }
        public FileBLOB(string name, byte[] data) {  Name = name; Data = data; }
        public FileBLOB(string name, byte[] data, BitmapImage bitmapImage)
        {
            Name = name; Data = data;
            this.BitmapImage = bitmapImage;
            this.BitmapImage = bitmapImage;
        }

    }
}
