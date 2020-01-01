namespace TestUtilities
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Collections.Generic;

  using Utilities;
  public static class GlobalApp
  {
    /// <summary>
    /// Aqui definimos la variable que va a guardar exactamente el archivo log generado desde el servicio de Windows ya que 
    /// si definimos la carpeta en donde se ejecuta el servicio, normalmente se generan los logs en %systemdrive%\system32 y no
    /// es aconsejable colocarlos ahí.
    /// </summary>
    public static string FolderApp = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

    /// <summary>
    /// Carpera de la cuenta local del usuario.
    /// </summary>
    /// <returns>Devuelve el nombre de la carpeta personal de la cuenta de usuario.</returns>
    public static string FolderPersonal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    /// <summary>
    /// Carpete de archivos de credenciales.
    /// </summary>
    /// <returns>Devuelve el nombre de la carpeta de las credenciales de la aplicación.</returns>
    public static string FolderCredentials = Path.Combine(FolderApp, @"Auth");

    /// <summary>
    /// Carpeta LOG de la aplicación.
    /// </summary>
    public static string FolderLog = Path.Combine(FolderApp, @"Log");

    /// <summary>
    /// Carpeta LAYOUT de la aplicación.
    /// </summary>
    public static string FolderLayOut = Path.Combine(FolderApp, @"Layout"); 
    
    /// <summary>
    /// Carpeta temporal.
    /// </summary>
    public static string FolderTemporal = Path.Combine(FolderApp, @"Temp"); 
  
    /// <summary>
    /// Numero de error.
    /// </summary>
    public static uint Numero = 0;

    /// <summary>
    /// Descripción del mensaje de error.
    /// </summary>
    public static string Mensaje = string.Empty;

    /// <summary>
    /// Lista de detalles de error.
    /// </summary>
    public static List<DetailLOG> DetailLog = null;

    /// <summary>
    /// El objeto 'LOGFiles' para almacenar los mensajes de error.
    /// </summary>
    public static LOGFiles oLog = null;

    /// <summary>
    /// Contador generico para el ordenamiento de los mensajes de error.
    /// </summary>
    public static int iCount = 0;

    /// <summary>
    /// Fecha actual.
    /// </summary>
    public static DateTime FechaActual;

    /// <summary>
    /// Valor de tiempo de sesión.
    /// </summary>
    public static uint TimeOutSession = 0;

    /// <summary>
    /// Valor de número de procesadores del equipo local.
    /// </summary>
    /// <returns>Devuelve un entero con el total de número de procesadores que tiene el equipo local donde se está ejecutando esta aplicación.</returns>
    public static uint NumeroProcesadores = (uint)Environment.ProcessorCount;

  }
}