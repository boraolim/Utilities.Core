using System;

namespace TestUtilities.DBSample.ContextDb
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public Guid StoreId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        public virtual Store Store { get; set; }
    }
}
