// Archivo: IWhereClauseGeneratorService.cs
// Interfaz 'IWhereClauseGeneratorService' para el armado de sentencias dinamicas SQL para la consulta a Base de Datos por medio del modelo ORM/Data.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.Threading.Tasks;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IWhereClauseGeneratorService' para el armado de sentencias dinamicas SQL para la consulta a Base de Datos por medio del modelo ORM/Data.
  /// </summary>
  public interface IWhereClauseGeneratorService
  {
    /// <summary>
    /// Genera un string con parametros de consulta a una tabla o entidad de la Base de Datos (para Dynamic Query Linq).
    /// </summary>
    /// <param name="Filtro">Objeto del tipo 'Filter'.</param>
    /// <param name="T">Estructura interna del tipo de dato</param>
    /// <returns>Una cadena que contiene el filtro armado en base a los atributos internos del objeto. Si no hay criterio de busqueda armado devuelve 'nulo' o 'vacio' debido a que no hay reglas o condiciones o bien una regla o condición contiene operadores incorrectos, según el tipo de dato del atributo o campo.</returns>
    Task<string> ParserFilterToDynamicLinqAsync<T>(WhereFilter Filtro) where T : new();

    /// <summary>
    /// Genera un string con parametros de consulta a una tabla o entidad de la Base de Datos (para Entity Framework).
    /// </summary>
    /// <param name="Filtro">Objeto del tipo 'Filter'.</param>
    /// <param name="T">Estructura interna del tipo de dato</param>
    /// <returns>Una cadena que contiene el filtro armado en base a los atributos internos del objeto. Si no hay criterio de busqueda armado devuelve 'nulo' o 'vacio' debido a que no hay reglas o condiciones o bien una regla o condición contiene operadores incorrectos, según el tipo de dato del atributo o campo.</returns>
    Task<string> ParserFilterToEntityFrameworkAsync<T>(WhereFilter Filtro) where T : new();
  }
}
