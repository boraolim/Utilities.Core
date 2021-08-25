// Archivo: Constants.cs
// Clase 'Constantes' que contiene valores definidos para la librería a nivel general.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
//© Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;

namespace Utilities
{
  /// <summary>
  /// Clase 'Constantes' que contiene valores definidos para la librería a nivel general.
  /// </summary>
  [Serializable]
  public class Constants
  {
    /// <summary>
    /// Valor en segundos mínimo para la pausa.
    /// </summary>
    public const int MINIMUMSECONDSLEEP = 2;

    /// <summary>
    /// Valor en segundos máximo para la pausa.
    /// </summary>
    public const int MAXIMUMSECONDSLEEP = 5;

    /// <summary>
    /// Valor del segundo en milisegundos.
    /// </summary>
    public const int MILISECONDSLEEP = 1000;

    /// <summary>
    /// Valor de todos los caracteres alfanumericos.
    /// </summary>
    public const string ALPHACHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_@$5&¿?¡!#=.*-[]{}abcdefghijklmnopqrstuvwxyz0123456789";
  }
}
