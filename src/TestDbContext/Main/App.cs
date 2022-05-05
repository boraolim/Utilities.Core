using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bogus;
using Utilities;

using TestUtilities.DBSample.ContextDb;

namespace TestUtilities.DBSample.Main
{
    /// <summary>
    /// Defines the <see cref="App" />.
    /// </summary>
    public class App
    {
        /// <summary>
        /// Defines the _context.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="AppDbContext"/>.</param>
        public App(AppDbContext context) => _context = context;

        /// <summary>
        /// The RunAsync.
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RunAsync(string[] args)
        {
            try
            {
                Console.Title = "Prueba de la librería Utilities.Core para .NET Core";

                using (var db = new AppDbContext())
                {
                    // var customers = GetCustomers();
                    var stores = new List<Store>()
                    {
                        new Store()
                        {
                            StoreId = Guid.NewGuid(),
                            OpenDate = new DateTime(2020, 12, 18),
                            StoreName = "Tractor Store",
                            TotalRevenue = 200000000,
                            Products = new List<Product>()
                            {
                                new Product()
                                {
                                    ProductName = "Model 12 Tractor",
                                    ProductPrice = 75000
                                },
                                new Product()
                                {
                                    ProductName = "Model 14 Tractor",
                                    ProductPrice = 122000
                                },
                                new Product()
                                {
                                    ProductName = "Plow",
                                    ProductPrice = 1200
                                }
                            }
                        },
                        new Store()
                        {
                            StoreId = Guid.NewGuid(),
                            OpenDate = new DateTime(2021, 12, 15),
                            StoreName = "Tire Store",
                            TotalRevenue = 150000000,
                            Products = new List<Product>()
                            {
                                new Product()
                                {
                                    ProductName = "Tractor Tire",
                                    ProductPrice = 12000
                                },
                                new Product()
                                {
                                    ProductName = "Car Tire",
                                    ProductPrice = 120
                                },
                                new Product()
                                {
                                    ProductName = "Cap",
                                    ProductPrice = 75
                                }
                            }
                        }
                    };

                    // _context.AddRange(customers); 
                    _context.AddRange(stores); await _context.SaveChangesAsync();

                    var FirstNameFilter = new WhereFilter
                    {
                        Condition = GroupOp.OR,
                        Rules = new List<WhereFilter>
                        {
                            new WhereFilter
                            {
                                Field = "CustomerGuid",
                                Operator = WhereConditionsOp.Contains,
                                Data = new[] { "885"}
                            },
                            new WhereFilter
                            {
                                Field = "Name",
                                Operator = WhereConditionsOp.Contains,
                                Data = new[] { "and" }
                            },
                            new WhereFilter
                            {
                                Field = "Email",
                                Operator = WhereConditionsOp.Contains,
                                Data = new[] { "yahoo" }
                            },
                            new WhereFilter
                            {
                                Field = "Address",
                                Operator = WhereConditionsOp.Contains,
                                Data = new[] { "5" }
                            }
                        }
                    };

                    var NamesFilteredList = _context.Customers.BuildQuery(FirstNameFilter).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildQuery (Clientes): {NamesFilteredList.Count}.");

                    var expressionPredicateCustomer = QueryBuilder.BuildPredicate<Customer>(FirstNameFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false });
                    var NamesFilteredList2 = _context.Customers.Where(expressionPredicateCustomer).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildPredicate (Clientes): {NamesFilteredList2.Count}.");

                    var expressionPredicateCustomer2 = QueryBuilder.BuildExpressionLambda<Customer>(FirstNameFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false });
                    var NamesFilteredList3 = _context.Customers.Where(expressionPredicateCustomer2).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildExpressionLambda (Clientes): {NamesFilteredList3.Count}.");

                    var tractorFilter = new WhereFilter
                    {
                        Condition = GroupOp.OR,
                        Rules = new List<WhereFilter>
                        {
                            new WhereFilter
                            {
                                Field = "ProductName",
                                Operator = WhereConditionsOp.Contains,
                                Data = new[] { "Tractor" }
                            }
                        }
                    };

                    var tractorIdFilteredList = _context.Products.BuildQuery(tractorFilter).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildQuery: {tractorIdFilteredList.Count}.");
                    var expressionPredicate = QueryBuilder.BuildPredicate<Product>(tractorFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false });
                    var tractorIdFilteredList2 = _context.Products.Where(expressionPredicate).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildPredicate: {tractorIdFilteredList2.Count}.");
                    var expressionPredicate2 = QueryBuilder.BuildExpressionLambda<Product>(tractorFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false });
                    var tractorIdFilteredList3 = _context.Products.Where(expressionPredicate2).ToList();
                    Console.WriteLine($"Total de registros obtenidos filtrados por BuildExpressionLambda: {tractorIdFilteredList3.Count}.");
                }
                Console.WriteLine("Proceso terminado.");
            }
            catch (Exception oEx)
            {
                Console.WriteLine($"{oEx.Message.Trim()}");
            }
            finally
            {
                Console.WriteLine("Pulse cualquier tecla para salir..."); Console.ReadLine();
            }
        }

        private IEnumerable<Customer> GetCustomers()
        {
            Randomizer.Seed = new Random(123456);

            var customerGenerator = new Faker<Customer>()
                .RuleFor(c => c.CustomerGuid, f => f.Random.Guid())
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.PostalZip, f => f.Address.ZipCode())
                .RuleFor(c => c.Country, f => f.Address.Country())
                .RuleFor(c => c.Amount, f => f.Finance.Random.Double(0, 5000));
            return customerGenerator.Generate(2500);
        }
    }
}
