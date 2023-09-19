using System.Collections.Generic;

namespace KoOrderRegister.Model
{
    public class CustomerProduct
    {
        public CustomerProduct() 
        { 
            Custumer = new Customer();
            this.Files = new List<FileBLOB>();
        }
        public CustomerProduct(Customer c)
        {
            Custumer = c;
            this.Files = new List<FileBLOB>();
        }
        public CustomerProduct(long productID, string orderNumber, string orderDate, string orderEelaseDate, string note, Customer custumer) 
        {
            this.ProductId = productID;
            this.OrderNumber = orderNumber; 
            this.OrderDate = orderDate;
            this.OrderReleaseDate = orderEelaseDate;
            this.Note = note;
            this.Custumer = custumer;
            this.Files = new List<FileBLOB>();

        }
        public Customer Custumer { get; set; }
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
