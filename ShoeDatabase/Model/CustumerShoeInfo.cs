using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeDatabase.Model
{
    public class CustomerShoeInfo
    {
        public CustomerShoeInfo() 
        { 
            Custumer = new Custumer(); 
        }

        public CustomerShoeInfo(int shoeId, string orderNumber, string orderDate, string orderEelaseDate, string photoPath, string note, Custumer custumer) 
        {
            this.ShoeId = shoeId;
            this.OrderNumber = orderNumber; 
            this.OrderDate = orderDate;
            this.OrderReleaseDate = orderEelaseDate;
            this.PhotoPath = photoPath; 
            this.Note = note;
            this.Custumer = custumer;

        }
        public Custumer Custumer { get; set; }
        public int ShoeId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderReleaseDate { get; set; }
        public string PhotoPath { get; set; }
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
