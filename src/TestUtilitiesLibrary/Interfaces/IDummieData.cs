using System.Collections.Generic;

namespace TestUtilitiesLibrary
{

  /// <summary>
  /// Interfaz 'IDummieData'.
  /// </summary>
  /// <summary xml:lang="es-MX">
  /// Interfaz 'IDummieData'.
  /// </summary>
  /// <summary xml:lang="en">
  /// 'IDummieData' interface.
  /// </summary>
  /// <summary xml:lang="en-US">
  /// 'IDummieData' interface.
  /// </summary>
  public interface IDummieData
  {
    /// <summary>
    /// Una función que muestra la lista de clientes.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Una función que muestra la lista de clientes.
    /// </summary>
    /// <summary xml:lang="en">
    /// A function that displays the list of customers.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// A function that displays the list of customers.
    /// </summary>
    public IEnumerable<Customer> GetCustomers();
  }
}
