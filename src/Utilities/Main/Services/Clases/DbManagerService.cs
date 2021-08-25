// Clase: DBManager.
// Clase 'DBManager' para administrar las operaciones de Bases de Datos.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Utilities.Resources;
using System.Collections;
using System.Text.RegularExpressions;

namespace Utilities
{
  /// <summary>
  /// Clase 'DBManager' para administrar las operaciones de Bases de Datos.
  /// </summary>
  [Serializable]
  public class DbManagerService : IDbManagerService
  {
    /* Objetos no administrados (variables locales a nivel de la clase). */
    protected ushort _intNumberErr;
    protected string _strMessage;
    protected ResourceManager _resourceData;

    /* Servicius internos de 'Utilities'. */
    protected IToolService _iToolService;

    /// <summary>
    /// Cadena de conexión de Base de Datos.
    /// </summary>
    internal string _cadenaConexion { get; set; }

    /// <summary>
    /// Proveedor de Base de Datos.
    /// </summary>
    internal DataBaseProviders _proveedorDatos { get; set; }

    /// <summary>
    /// Objeto IDatabaseHandler.
    /// </summary>
    internal IDatabaseHandler _database { get; set; }

    #region "Atributos."

    /* Asignando atributos a la interfaz. */
    public string CadenaConexion { get => _cadenaConexion; set => _cadenaConexion = value; }
    public DataBaseProviders ProveedorDatos { get => _proveedorDatos; set => _proveedorDatos = value; }

    #endregion

    /// <summary>
    /// Constructor de la clase 'GoogleRepositoryService'.
    /// </summary>
    /// <param name="dictionaryCollectionService">Objeto del tipo 'IDictionaryCollectionService'.</param>
    public DbManagerService(IToolService toolService) => _iToolService = toolService;

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty;
      _resourceData = new ResourceManager(typeof(LanguageSource));

      /* Creamos la conexión a Base de Datos. */
      _database = CreateInstanceDataBase();
    }

    public IDbDataParameter CreateParameter(string nameParameter, object valueObject, DbType dbType) => DataParameterManager.CreateParameter(ProveedorDatos, nameParameter, valueObject, dbType, ParameterDirection.Input);

    public IDbDataParameter CreateParameter(string nameParameter, object valueObject, int size, DbType dbType) => DataParameterManager.CreateParameter(ProveedorDatos, nameParameter, valueObject, size, dbType, ParameterDirection.Input);

    public IDbDataParameter CreateParameter(string nameParameter, object valueObject, int size, DbType dbType, ParameterDirection direction) => DataParameterManager.CreateParameter(ProveedorDatos, nameParameter, valueObject, size, dbType, direction);

    public async Task<DataTable> GetDataToDataTableAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); var dataset = new DataSet();

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 8901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                var dataAdaper = _database.CreateAdapter(command);
                dataAdaper.Fill(dataset);
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch(Exception oEx)
      {
        _intNumberErr = 8900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return dataset.Tables[0];
    }

    public async Task<DataSet> GetDataToDataSet(string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); var dataset = new DataSet();

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                var dataAdaper = _database.CreateAdapter(command);
                dataAdaper.Fill(dataset);
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return dataset;
    }

    public async Task<T> GetDataToMappingToSingleAsync<T>(string commandText, CommandType commandType, IDbDataParameter[] parameters = null) where T : class, new()
    {
      InitVars(); var dataset = new DataSet(); var _oRetSingle = new T();

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          using (var connection = _database.CreateConnection())
          {
            connection.Open();

            using (var command = _database.CreateCommand(commandText, commandType, connection))
            {
              if (parameters != null)
              {
                foreach (var parameter in parameters)
                  command.Parameters.Add(parameter);
              }

              var dataAdaper = _database.CreateAdapter(command);
              dataAdaper.Fill(dataset);

              /* Validamos si realmente devuelve un registro. */
              if (dataset.Tables[0].Rows.Count > 1)
              {
                _intNumberErr = 9102;
                _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryToSingleRequired")}";
              }
              else if (dataset.Tables[0].Rows.Count == 1)
              {
                /* Hacemos el mapping del DataTable a List<T>. */
                var _newList = await _iToolService.ConvertDataTableAsync<T>(dataset.Tables[0]);
                _oRetSingle = _newList.SingleOrDefault();
              }
              else
              {
                _oRetSingle = null;
              }
            } /* Cierro el objeto command. */
          } /* Cierro la conexión a Base de Datos. */
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9100;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oRetSingle;
    }

    public async Task<List<T>> GetDataToMappingAsync<T>(string commandText, CommandType commandType, IDbDataParameter[] parameters = null) where T : class, new()
    {
      InitVars(); var dataset = new DataSet(); var _oRetList = new List<T>();

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          using (var connection = _database.CreateConnection())
          {
            connection.Open();

            using (var command = _database.CreateCommand(commandText, commandType, connection))
            {
              if (parameters != null)
              {
                foreach (var parameter in parameters)
                  command.Parameters.Add(parameter);
              }

              var dataAdaper = _database.CreateAdapter(command);
              dataAdaper.Fill(dataset);

              /* Hacemos el mapping del DataTable a List<T>. */
              _oRetList = await _iToolService.ConvertDataTableAsync<T>(dataset.Tables[0]);

            } /* Cierro el objeto command. */
          } /* Cierro la conexión a Base de Datos. */
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9200;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oRetList;
    }

    public async Task<string> GetDataToJSONAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); var dataset = new DataSet(); var _oRetList = string.Empty;

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                var dataAdaper = _database.CreateAdapter(command);
                dataAdaper.Fill(dataset);

                /* Hacemos el mapping del DataTable a formato JSON. */
                _oRetList = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dataset.Tables[0], Formatting.None)));

              } /* Cierro el objeto command. */
            } /* Cierro la conexión a Base de Datos. */

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oRetList;
    }

    public async Task<string> GetDataTojqGridJSONAsync(string titulo, string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); var dataset = new DataSet(); var _oRetList = string.Empty;
      var _oSbColModel = new StringBuilder(); var _oSbDataModel = new StringBuilder(); var _oRetJqT = new jqGridTemplate();

      try
      {
        if (string.IsNullOrEmpty(titulo) == true | titulo.Length == 0)
        {
          _intNumberErr = 9401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strJqGridTitleRequired")}";
        }
        else if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                var dataAdaper = _database.CreateAdapter(command);
                dataAdaper.Fill(dataset);

                /* Hacemos el mapping del DataTable a formato JSON. */
                // Si el DataSet trajó registros...
                if (dataset.Tables[0].Rows.Count > 0)
                {
                  foreach (DataColumn dc in dataset.Tables[0].Columns)
                  {
                    var _iSizeField = 0;

                    switch (dc.DataType.ToString())
                    {
                      case "System.SByte":    // tinyint
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (120 + _iSizeField));
                        break;
                      case "System.Int16":    // smallint
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (180 + _iSizeField));
                        break;
                      case "System.Int32":    // int
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (120 + _iSizeField));
                        break;
                      case "System.Int64":    // bigint
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (120 + _iSizeField));
                        break;
                      case "System.Single":   // Edm.Single, in SQL: float
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'right', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (180 + _iSizeField));
                        break;
                      case "System.Double":   // float(53), double precision
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'right', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (180 + _iSizeField));
                        break;
                      case "System.Boolean":
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'right', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (120 + _iSizeField));
                        break;
                      case "System.DateTime": // smalldatetime, datetime
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (180 + _iSizeField));
                        break;
                      case "System.String":   // nvarchar, varchar, char
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'center', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (150 + _iSizeField));
                        break;
                      default:
                        _oSbColModel.AppendFormat("&#123; name: '{0}', label: '{1}', width: {2}, align: 'right', sortable: true, resizable: true, hidden: false, frozen: true, search: false, editable: false &#125;&#44; ", dc.ColumnName.Trim(), dc.Caption.Trim(), (180 + _iSizeField));
                        break;
                    }
                  } // Fin del bucle foreach para el recorrido.

                  /* Construyo entonces el objeto colModel y serializo el array de datos. */
                  var _oColModel = JsonConvert.DeserializeObject<List<colModel>>(string.Concat("[ ", _oSbColModel.ToString().Replace("&#123;", "{").Replace("&#125;&#44;", "},"), " ]"));
                  var _oData = JsonConvert.DeserializeObject<ArrayList>(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dataset.Tables[0], Formatting.None))));

                  /* Armado del componente final 'jqGridTemplate'. */
                  _oRetJqT.caption = titulo.Trim(); _oRetJqT.colModel = _oColModel; _oRetJqT.data = _oData;

                  /* Finalmente convierto en JSON string el objeto jqGridTemplate. */
                  _oRetList = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_oRetJqT, Formatting.None)));

                } // Fin de la validación de registros.

              } /* Cierro el objeto command. */
            } /* Cierro la conexión a Base de Datos. */

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9400;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oRetList;
    }

    public async Task ExportDataAsync(string strFileName, string strSeparator, string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); var dataset = new DataSet(); var _oRetList = string.Empty;

      try
      {
        if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 9501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strFileName, @"^.*\.(txt|csv|dat)"))
        {
          _intNumberErr = 9502;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strCSVExtensionRequired")}";
        }
        else if (Regex.IsMatch(strSeparator, @"^.*\.(\||,|\t)$"))
        {
          _intNumberErr = 9503;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSeparatorRequired")}";
        }
        else if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9504;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          using (var connection = _database.CreateConnection())
          {
            connection.Open();

            using (var command = _database.CreateCommand(commandText, commandType, connection))
            {
              if (parameters != null)
              {
                foreach (var parameter in parameters)
                  command.Parameters.Add(parameter);
              }

              var dataAdaper = _database.CreateAdapter(command);
              dataAdaper.Fill(dataset);

              /* Hacemos el mapping del DataTable a formato JSON. */
              await _iToolService.DataTableToCSVAsync(dataset.Tables[0], strFileName, strSeparator);

            } /* Cierro el objeto command. */
          } /* Cierro la conexión a Base de Datos. */
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9500;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task ExecuteSQLAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                /* Ejecutamos la sentencia SQL. */
                command.ExecuteNonQuery();
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9600;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task ExecuteSQLWithTransactionAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters)
    {
      InitVars(); IDbTransaction transactionScope = null;

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open(); transactionScope = connection.BeginTransaction();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                /* Ejecutamos la sentencia SQL. */
                try
                {
                  command.ExecuteNonQuery(); transactionScope.Commit();
                }
                catch (Exception)
                {
                  _intNumberErr = 9702;
                  _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLTransactionFailed")}";
                  transactionScope.Rollback();
                }
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task ExecuteSQLWithTransactionAsync(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters)
    {
      InitVars(); IDbTransaction transactionScope = null;

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open(); transactionScope = connection.BeginTransaction(isolationLevel);

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                /* Ejecutamos la sentencia SQL. */
                try
                {
                  command.ExecuteNonQuery(); transactionScope.Commit();
                }
                catch (Exception)
                {
                  _intNumberErr = 9802;
                  _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLTransactionFailed")}";
                  transactionScope.Rollback();
                }
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task<object> GetScalarValueAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null)
    {
      InitVars(); object _objValue = null;

      try
      {
        if (string.IsNullOrEmpty(commandText) == true | commandText.Length == 0)
        {
          _intNumberErr = 9901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSQLQueryStringRequired")}";
        }
        else
        {
          /* Realizamos la conexión a la Base de Datos y generamos el set de datos. */
          await Task.Run(() =>
          {
            using (var connection = _database.CreateConnection())
            {
              connection.Open();

              using (var command = _database.CreateCommand(commandText, commandType, connection))
              {
                if (parameters != null)
                {
                  foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                }

                /* Ejecutamos la sentencia SQL. */
                _objValue = command.ExecuteScalar();
              }
            } // Cierro la conexión a Base de Datos.

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 9900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _objValue;
    }

    #region "Funciones privadas."

    /// <summary>
    /// Función que crea una instancia de la Base de Datos tomada del archivo de configuración de la aplicación.
    /// </summary>
    /// <returns>Devuelve un objeto 'IDataBaseHandler' que contiene un objeto IDbConnection con la instancia de Base de Datos cargada y configurada previamente.</returns>
    protected IDatabaseHandler CreateInstanceDataBase()
    {
      IDatabaseHandler _database = null;

      switch (ProveedorDatos)
      {
        case DataBaseProviders.SQLServer:                                          // Conexión a SQL Server.
          _database = new SQLServerConnection();
          break;
        case DataBaseProviders.Odbc:                                               // Conexión a ODBC.
          _database = new ODBCConnection();
          break;
        case DataBaseProviders.MySQLServer:                                        // Conexión a MySQL/MariaDB.
          _database = new MySQLServerConnection();
          break;
      }

      _database.CadenaConexion = CadenaConexion;
      return _database;
    }

    #endregion
  }
}
