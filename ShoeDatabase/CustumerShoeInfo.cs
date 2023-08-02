using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeDatabase
{
    public class CustomerShoeInfo
    {
        public int ShoeId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderReleaseDate { get; set; }
        public string PhotoPath { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TajNumber { get; set; }
        public int CustomerId { get; set; }
    }

}
