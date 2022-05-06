using System;

using Utilities;

namespace TestUtilitiesLibrary
{
    /// <summary>
    /// Clase 'SysObjects'.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Interfaz 'SysObjects'.
    /// </summary>
    /// <summary xml:lang="en">
    /// 'SysObjects' class.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// 'SysObjects' class.
    /// </summary>
    public class SysObjects
    {
        /// <summary>
        /// Atributo 'name'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'name'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'name' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'name' attribute.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Atributo 'id'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'id'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'id' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'id' attribute.
        /// </summary>
        public int id { get; set; }
    }

    /// <summary>
    /// Clase 'Order'.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Interfaz 'Order'.
    /// </summary>
    /// <summary xml:lang="en">
    /// 'Order' class.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// 'Order' class.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Atributo 'Id'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Id'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Id' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Id' attribute.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Atributo 'Date'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Date'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Date' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Date' attribute.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Atributo 'OrderValue'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'OrderValue'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'OrderValue' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'OrderValue' attribute.
        /// </summary>
        public Decimal OrderValue { get; set; }

        /// <summary>
        /// Atributo 'Shipped'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Shipped'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Shipped' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Shipped' attribute.
        /// </summary>
        public bool Shipped { get; set; }
    }

    /// <summary>
    /// Clase 'Customer'.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Interfaz 'Customer'.
    /// </summary>
    /// <summary xml:lang="en">
    /// 'Customer' class.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// 'Customer' class.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Atributo 'Id'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Id'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Id' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Id' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Id", DataType = typeof(Guid))]
        public Guid Id { get; set; }

        /// <summary>
        /// Atributo 'Name'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Name'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Name' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Name' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Name", DataType = typeof(string))]
        public string Name { get; set; }

        /// <summary>
        /// Atributo 'Address'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Address'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Address' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Address' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Address", DataType = typeof(string))]
        public string Address { get; set; }

        /// <summary>
        /// Atributo 'City'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'City'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'City' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'City' attribute.
        /// </summary>
        [ColumnAttribute(Name = "City", DataType = typeof(string))]
        public string City { get; set; }

        /// <summary>
        /// Atributo 'Address'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Address'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Address' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Address' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Country", DataType = typeof(string))]
        public string Country { get; set; }

        /// <summary>
        /// Atributo 'ZipCode'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'ZipCode'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'ZipCode' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'ZipCode' attribute.
        /// </summary>
        [ColumnAttribute(Name = "ZipCode", DataType = typeof(string))]
        public string ZipCode { get; set; }

        /// <summary>
        /// Atributo 'Phone'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Phone'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Phone' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Phone' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Phone", DataType = typeof(string))]
        public string Phone { get; set; }

        /// <summary>
        /// Atributo 'Email'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'Email'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'Email' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'Email' attribute.
        /// </summary>
        [ColumnAttribute(Name = "Email", DataType = typeof(string))]
        public string Email { get; set; }

        /// <summary>
        /// Atributo 'ContactName'.
        /// </summary>
        /// <summary xml:lang="es-MX">
        /// Atributo 'ContactName'.
        /// </summary>
        /// <summary xml:lang="en">
        /// 'ContactName' attribute.
        /// </summary>
        /// <summary xml:lang="en-US">
        /// 'ContactName' attribute.
        /// </summary>
        [ColumnAttribute(Name = "ContactName", DataType = typeof(string))]
        public string ContactName { get; set; }
        // public IEnumerable<Order> Orders { get; set; }
    }
}
