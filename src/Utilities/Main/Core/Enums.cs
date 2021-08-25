// Archivo: Enums.cs
// Colección de enumeraciones para las funciones de la librería Utilities.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 3 de julio de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Utilities
{
  /// <summary>
  /// Enumeración 'StatusAccunt' de estatus de cuentas de usuario.
  /// </summary>
  public enum StatusAccunt
  {
    /// <summary>
    /// Usuario autenticado.
    /// </summary>
    Logged = 0,

    /// <summary>
    /// Contraseña erronea.
    /// </summary>
    PasswordIncorrect = 1,

    /// <summary>
    /// Usuario no existe.
    /// </summary>
    NotExistsUser = 2,

    /// <summary>
    /// Usuario inactivo o bloqueado.
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Usuario sin permisos para la aplicación.
    /// </summary>
    NotPrivileges = 4,

    /// <summary>
    /// Fallo en la autenticación.
    /// </summary>
    FailedAuth = 5
  }

  /// <summary>
  /// Enumeración 'WhereConditions' para almacenar los valores de condicion para la clausula WHERE del lenguaje SQL.
  /// </summary>
  [Serializable]
  public enum WhereConditions
  {
    /// <summary>
    /// Condicion IGUAL.
    /// </summary>
    eq, // "equal (igual)"

    /// <summary>
    /// Condicion DISTINTO o NO IGUAL.
    /// </summary>
    ne, // "not equal (no igual o distinto)"

    /// <summary>
    /// Condición MENOR.
    /// </summary>
    lt, // "less (menor)"

    /// <summary>
    /// Condición MENOR QUE.
    /// </summary>
    le, // "less or equal (menor igual que)"

    /// <summary>
    /// Condición MAYOR.
    /// </summary>
    gt, // "greater (mayor)"

    /// <summary>
    /// Condición MAYOR IGUAL QUE.
    /// </summary>
    ge, // "greater or equal (mayor igual que)"

    /// <summary>
    /// Condicion EMPIEZA CON.
    /// </summary>
    bw, // "begins with (empieza con)"

    /// <summary>
    /// Condición NO EMPIEZA CON.
    /// </summary>
    bn, // "does not begin with (no empieza con)"

    //in,   // "in (en)"
    //ni,   // "not in (no tenga)"

    /// <summary>
    /// Condición TERMINA EN.
    /// </summary>
    ew, // "ends with (termina en)"

    /// <summary>
    /// Condición NO TERMINA EN.
    /// </summary>
    en, // "does not end with (no termina en)"

    /// <summary>
    /// Condición CONTIENE.
    /// </summary>
    cn, // "contains (contiene)"

    /// <summary>
    /// Condición NO CONTIENE.
    /// </summary>
    nc // "does not contain (no contiene)"

    //nu,   // "is null (es nulo)"
    //nn    // "not null (no es nulo)"
  };


  /// <summary>
  /// Enumeración 'TipoInformacion' que indica los tipos de errores que se pueden registrar en el LOG.
  /// </summary>
  [Serializable]
  public enum TipoInformacion
  {
    /// <summary>
    /// Status = Información.
    /// </summary>
    Informacion = 0,

    /// <summary>
    /// Status = Advertencia.
    /// </summary>
    Advertencia = 1,

    /// <summary>
    /// Status = Falla.
    /// </summary>
    Falla = 2,

    /// <summary>
    /// Status = ErrorProceso.
    /// </summary>
    ErrorProceso = 3
  };

  /// <summary>
  /// Enumeración 'OperatorComparer' que indica los tipos de condiciones lógicas.
  /// </summary>
  [Serializable]
  public enum OperatorComparer
  {
    /// <summary>
    /// Contiene algo...
    /// </summary>
    Contains,

    /// <summary>
    /// Inicia o empieza con...
    /// </summary>
    StartsWith,

    /// <summary>
    /// Termina o finaliza en...
    /// </summary>
    EndsWith,

    /// <summary>
    /// Igual a...
    /// </summary>
    Equals = ExpressionType.Equal,

    /// <summary>
    /// Mayor que...
    /// </summary>
    GreaterThan = ExpressionType.GreaterThan,

    /// <summary>
    /// Mayor o igual que...
    /// </summary>
    GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,

    /// <summary>
    /// Menor que...
    /// </summary>
    LessThan = ExpressionType.LessThan,

    /// <summary>
    /// Menor igual que...
    /// </summary>
    LessThanOrEqual = ExpressionType.LessThan,

    /// <summary>
    /// Distinto de...
    /// </summary>
    NotEqual = ExpressionType.NotEqual
  }

  /// <summary>
  /// Enumeración 'GroupOp' para almacenar los valores booleanos de decisión.
  /// </summary>
  [Serializable]
  public enum GroupOp
  {
    /// <summary>
    /// Operador AND
    /// </summary>
    AND,

    /// <summary>
    /// Operador OR
    /// </summary>
    OR
  };

  /// <summary>
  /// Enumeración 'EnumRequestType' para la enumeración de tipos de verbos HTTP.
  /// </summary>
  [Serializable]
  public enum EnumRequestType
  {
    /// <summary>
    /// Petición GET
    /// </summary>
    GET,

    /// <summary>
    /// Petición POST
    /// </summary>
    POST,

    /// <summary>
    /// Petición PUT
    /// </summary>
    PUT,

    /// <summary>
    /// Petición DELETE
    /// </summary>
    DELETE,

    /// <summary>
    /// Petición PATCH
    /// </summary>
    PATCH,

    /// <summary>
    /// Petición HEADER
    /// </summary>
    HEADER
  }

  /// <summary>
  /// Enumeración 'DataBaseProviders' para almacenar los valores de tipos de plataforma de Base de Datos.
  /// </summary>
  [Serializable]
  public enum DataBaseProviders
  {
    /// <summary>
    /// Ninguna fuente de Base de Datos.
    /// </summary>
    Ninguno = 0,

    /// <summary>
    /// SQLite.
    /// </summary>
    SQLite = 1,

    /// <summary>
    /// Microsoft SQL Server.
    /// </summary>
    SQLServer = 2,

    /// <summary>
    /// MySQL Server / Maria DB Server.
    /// </summary>
    MySQLServer = 3,

    /// <summary>
    /// PostgreSQL.
    /// </summary>
    PostgreSQL = 4,

    /// <summary>
    /// Orale DataBase.
    /// </summary>
    Oracle = 5,

    /// <summary>
    /// ODBC.
    /// </summary>
    Odbc = 7
  }

  /// <summary>
  /// Enumeración de grupos de acceso en Google Drive.
  /// </summary>
  public enum GoogleDriveGroups
  {
    /// <summary>
    /// Usuario.
    /// </summary>
    User,

    /// <summary>
    /// Grupo de dominio.
    /// </summary>
    Group,

    /// <summary>
    /// Dominio.
    /// </summary>
    Domain,

    /// <summary>
    /// Por defecto.
    /// </summary>
    Default
  }

  /// <summary>
  /// Enumeración de permisos de lectura en Google Drive.
  /// </summary>
  public enum GoogleDrivePermissions
  {
    /// <summary>
    /// Dueño o propietario.
    /// </summary>
    Owner,

    /// <summary>
    /// Lectura.
    /// </summary>
    Reader,

    /// <summary>
    /// Escritura.
    /// </summary>
    Writter
  }

  /// <summary>
  /// Enumeración de modo de acceso a Google API.
  /// </summary>
  public enum GoogleAPIModeAccessServiceAccount
  {
    /// <summary>
    /// Acceso por archivo de credenciales tipo JSON.
    /// </summary>
    ByJSONFile,

    /// <summary>
    /// Acceso por archivo de credenciales tipo key.
    /// </summary>
    ByKeyFile
  }
}
