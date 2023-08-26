using System.Collections.Generic;

namespace ShoeDatabase.Model
{
    public class CustomerShoeInfo
    {
        public CustomerShoeInfo() 
        { 
            Custumer = new Custumer(); 
        }

        public CustomerShoeInfo(int shoeId, string orderNumber, string orderDate, string orderEelaseDate, string note, Custumer custumer) 
        {
            this.ProductId = shoeId;
            this.OrderNumber = orderNumber; 
            this.OrderDate = orderDate;
            this.OrderReleaseDate = orderEelaseDate;
            this.Note = note;
            this.Custumer = custumer;

        }
        public Custumer Custumer { get; set; }
        public int ProductId { get; set; }
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
        public int CustomerId 
        { 
            get => Custumer.Id;
            set
            {
                Custumer.Id = value;
            }
        }
    
    }

}
