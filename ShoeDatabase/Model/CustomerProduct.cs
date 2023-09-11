using System.Collections.Generic;

namespace ShoeDatabase.Model
{
    public class CustomerProduct
    {
        public CustomerProduct() 
        { 
            Custumer = new Custumer();
            this.Files = new List<FileBLOB>();
        }
        public CustomerProduct(Custumer c)
        {
            Custumer = c;
            this.Files = new List<FileBLOB>();
        }
        public CustomerProduct(long productID, string orderNumber, string orderDate, string orderEelaseDate, string note, Custumer custumer) 
        {
            this.ProductId = productID;
            this.OrderNumber = orderNumber; 
            this.OrderDate = orderDate;
            this.OrderReleaseDate = orderEelaseDate;
            this.Note = note;
            this.Custumer = custumer;
            this.Files = new List<FileBLOB>();

        }
        public Custumer Custumer { get; set; }
        public long ProductId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderReleaseDate { get; set; }
        public string FileName { get; set; }
        public  List<FileBLOB> Files { get; set; }
        public string Note { get; set; }
        public string Name 
        { 
            get => Custumer.Name;
            set
            {
                Custumer.Name = value;
            } 
        }
        public string Address 
        { 
            get => Custumer.Address;
            set
            {
                Custumer.Address = value;
            } 
        }
        public string TajNumber 
        { 
            get => Custumer.TAJNumber;
            set
            {
                Custumer.TAJNumber = value;
            } 
        }
        public long CustomerId 
        { 
            get => Custumer.Id;
            set
            {
                Custumer.Id = value;
            }
        }
    
    }

}
