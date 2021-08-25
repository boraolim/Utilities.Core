// Archivo: IDataBaseHandler.
// Interfaz 'IDataBaseHandler' que contiene las funciones básicas para acceso a Bases de Datos basados en ADO.NET tradicional.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

namespace Utilities
{
  using System.Data;

  /// <summary>
  /// Interfaz 'IDataBaseHandler' que contiene las funciones básicas para acceso a Bases de Datos basados en ADO.NET tradicional.
  /// </summary>
  public interface IDatabaseHandler
  {
    /// <summary>
    /// Cadena de conexión extraída del nombre de la sección del archivo de configuración.
    /// </summary>
    public string CadenaConexion { get; set; }

    /// <summary>
    /// Método para crear un objeto conexión.
    /// </summary>
    /// <returns>Devuelve un objeto del tipo 'IDbConnection'.</returns>
    IDbConnection CreateConnection();

    /// <summary>
    /// Método para cerrar una conexión activa.
    /// </summary>
    /// <param name="connection">Objeto conexión.</param>
    void CloseConnection(IDbConnection connection);

    /// <summary>
    /// Método para crear un comando para Base de Datos.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando SQL.</param>
    /// <param name="connection">Objeto conexión.</param>
    /// <returns>Devuelve un objeto del tipo IDbCommand.</returns>
    IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection);

    /// <summary>
    /// Crear un objeto Adapter.
    /// </summary>
    /// <param name="command">Objeto comando</param>
    /// <returns>Devuelve un objeto del tipo IDataAdapter.</returns>
    IDataAdapter CreateAdapter(IDbCommand command);

    /// <summary>
    /// Crear un objeto Parameter.
    /// </summary>
    /// <param name="command">Objeto comando.</param>
    /// <returns>Devuelve un objeto del tipo IDbDataParameter.</returns>
    IDbDataParameter CreateParameter(IDbCommand command);
  }
}
