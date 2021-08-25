// Archivo: Exceptions.cs
// Colección de objetos heredados del tipo "System.Exception" para las funciones de la librería Utilities.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 3 de julio de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;

namespace Utilities
{
  /// <summary>
  /// Clase 'UtilitiesException' para el manejo de control de excepciones de la librería 'Utilities'.
  /// </summary>
  [Serializable]
  public class UtilitiesException : Exception
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="UtilitiesException"/> class.
    /// </summary>
    public UtilitiesException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UtilitiesException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public UtilitiesException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UtilitiesException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public UtilitiesException(string message, Exception innerException) :
      base(message, innerException)
    { }

    /// <summary>
    /// Constructor needed for serialization when exception propagates from a remoting server to the client.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    internal UtilitiesException(System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
    { }

  }
}
