using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ShoeDatabase.Model
{
    public class FileBLOB : INotifyPropertyChanged
    {
        private string _name;
        private byte[] _data;
        private long _id;
        private long _productId;
        private BitmapImage _bitmapImage;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    OnPropertyChanged(nameof(Data));
                }
            }
        }

        public long ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        public long ProductID
        {
            get { return _productId; }
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                    OnPropertyChanged(nameof(ProductID));
                }
            }
        }

        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                if (_bitmapImage != value)
                {
                    _bitmapImage = value;
                    OnPropertyChanged(nameof(BitmapImage));
                }
            }
        }

        public FileBLOB() { }

        public FileBLOB(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public FileBLOB(string name, byte[] data, BitmapImage bitmapImage)
        {
            Name = name;
            Data = data;
            BitmapImage = bitmapImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
