using System;
using System.Collections.Generic;

namespace TestUtilities.DBSample.ContextDb
{
    public partial class Store
    {
        public Store()
        {
            Products = new HashSet<Product>();
        }

        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime OpenDate { get; set; }
        public decimal TotalRevenue { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
