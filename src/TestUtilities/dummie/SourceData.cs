namespace TestUtilities
{
  using System;
  using System.Linq;
  using System.Collections.Generic;

  using Bogus;
  using Utilities;

  public class SysObjects
  {
    public string name { get; set; }
    public int id { get; set; }
  }

  public class Order
  {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Decimal OrderValue { get; set; }
    public bool Shipped { get; set; }
  }

  public class Customer
  {
    [ColumnAttribute(Name = "Id", DataType = typeof(Guid))]
    public Guid Id { get; set; }

    [ColumnAttribute(Name = "Name", DataType = typeof(string))]
    public string Name { get; set; }

    [ColumnAttribute(Name = "Address", DataType = typeof(string))]
    public string Address { get; set; }

    [ColumnAttribute(Name = "City", DataType = typeof(string))]
    public string City { get; set; }

    [ColumnAttribute(Name = "Country", DataType = typeof(string))]
    public string Country { get; set; }

    [ColumnAttribute(Name = "ZipCode", DataType = typeof(string))]
    public string ZipCode { get; set; }

    [ColumnAttribute(Name = "Phone", DataType = typeof(string))]
    public string Phone { get; set; }

    [ColumnAttribute(Name = "Email", DataType = typeof(string))]
    public string Email { get; set; }

    [ColumnAttribute(Name = "ContactName", DataType = typeof(string))]
    public string ContactName { get; set; }
    // public IEnumerable<Order> Orders { get; set; }
  }

  public interface IDummieData
  {
    public IEnumerable<Customer> GetCustomers();
  }
  public class DummieData : IDummieData
  {
    public IEnumerable<Customer> GetCustomers()
    {
      Randomizer.Seed = new Random(123456);

      //var ordergenerator = new Faker<Order>()
      //    .RuleFor(o => o.Id, Guid.NewGuid)
      //    .RuleFor(o => o.Date, f => f.Date.Past(3))
      //    .RuleFor(o => o.OrderValue, f => f.Finance.Amount(0, 10000))
      //    .RuleFor(o => o.Shipped, f => f.Random.Bool(0.9f));

      var customerGenerator = new Faker<Customer>()
          .RuleFor(c => c.Id, Guid.NewGuid())
          .RuleFor(c => c.Name, f => f.Company.CompanyName())
          .RuleFor(c => c.Address, f => f.Address.FullAddress())
          .RuleFor(c => c.City, f => f.Address.City())
          .RuleFor(c => c.Country, f => f.Address.Country())
          .RuleFor(c => c.ZipCode, f => f.Address.ZipCode())
          .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
          .RuleFor(c => c.Email, f => f.Internet.Email())
          .RuleFor(c => c.ContactName, (f, c) => f.Name.FullName());
          // .RuleFor(c => c.Orders, f => ordergenerator.Generate(f.Random.Number(10)).ToList());
      return customerGenerator.Generate(250);
    }
  }
}
