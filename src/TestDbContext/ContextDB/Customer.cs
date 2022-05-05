using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestUtilities.DBSample.ContextDb
{
    public partial class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }
        public Guid CustomerGuid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostalZip { get; set; }
        public string Country { get; set; }
        public double Amount { get; set; }
        public string List { get; set; }
    }
}
