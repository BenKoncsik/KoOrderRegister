using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KoOrderRegister.Model
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

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        public BitmapImage BitmapImage
        {
            get
            {
                if (_bitmapImage == null && !string.IsNullOrEmpty(_filePath))
                {
                    _bitmapImage = new BitmapImage(new Uri(_filePath));
                }
                return _bitmapImage;
            }
            set
            {
                if (_bitmapImage != value)
                {
                    _bitmapImage = value;
                    OnPropertyChanged(nameof(BitmapImage));
                }
            }
        }

        public FileBLOB(string name, byte[] data, string filePath)
        {
            Name = name;
            Data = data;
            FilePath = filePath;
        }

        public FileBLOB(string name, byte[] data, Uri imageUri)
        {
            Name = name;
            Data = data;

            _bitmapImage = new BitmapImage();
            _bitmapImage.BeginInit();
            _bitmapImage.UriSource = imageUri;
            _bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            _bitmapImage.EndInit();

            if (!_bitmapImage.IsFrozen && _bitmapImage.CanFreeze)
            {
                _bitmapImage.Freeze();
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
