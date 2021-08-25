// Archivo: IGoogleRepositoryService
// Interfaz 'IGoogleRepositoryService' para las operaciones de Google Drive y Google Sheets.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

/* Librerías de Google Apis. */
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IGoogleRepositoryService' para las operaciones de Google Drive y Google Sheets.
  /// </summary>
  public interface IGoogleRepositoryService
  {
    /// <summary>
    /// Ubicación física de la carpeta de autenticación de Google.
    /// </summary>
    public string FolderAuth { get; set; }
    /// <summary>
    /// Token de ubicación de Google API.
    /// </summary>
    public string UbicacionToken { get; set; }

    /// <summary>
    /// Clave del Id. Google asociada a la cuenta de Google.
    /// </summary>
    public string IdKeyGoogleDrive { get; set; }

    /// <summary>
    /// Identificador del permiso ID de la cuenta de Google.
    /// </summary>
    public string PermissionsId { get; set; }

    /// <summary>
    /// Recurso de Google Drive.
    /// </summary>
    public string URLSourceDrive { get; set; }

    /// <summary>
    /// Servicio de Google Drive v3.
    /// </summary>
    public Google.Apis.Drive.v3.DriveService GoogleDriveRepositoryV3 { get; set; }

    /// <summary>
    /// Servicio de Google Drive v2.
    /// </summary>
    public Google.Apis.Drive.v2.DriveService GoogleDriveRepositoryV2 { get; set; }

    /// <summary>
    /// Servicio de Google Sheets v4.
    /// </summary>
    public SheetsService GoogleSheetRepositoryv4 { get; set; }

    #region "Google Drive."

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz).
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta que se va a crear en Google Drive.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera una carpeta nueva en Google Drive (raíz). Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderName, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz) con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta que se va a crear en Google Drive.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera una carpeta nueva en Google Drive (raíz). Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderName, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz) con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta que se va a crear en Google Drive.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera una carpeta nueva en Google Drive (raíz). Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz)  con permisos para un rol y/o grupo.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta nueva.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <remarks>Genera un nuevo identificador del recurso en Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task CreateFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz)  con permisos para un rol y/o grupo con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta nueva.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <remarks>Genera un nuevo identificador del recurso en Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task CreateFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en Google Drive (raíz) con permisos para un rol y/o grupo con clave secreta del cliente.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderName">Nombre de la carpeta que se va a crear en Google Drive.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera una carpeta nueva en Google Drive (raíz). Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en una carpeta de Google Drive (subdirectorio).
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del recurso en Google Drive.</param>
    /// <param name="FolderName">Nombre de la subcarpeta.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo subdirectorio en Google Drive. Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateSubFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string FolderName, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en una carpeta de Google Drive (subdirectorio) con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del recurso en Google Drive.</param>
    /// <param name="FolderName">Nombre de la subcarpeta.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo subdirectorio en Google Drive. Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateSubFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string FolderName, bool IsCanShare);

    /// <summary>
    /// Función que crea una carpeta en una carpeta de Google Drive (subdirectorio) con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del recurso en Google Drive.</param>
    /// <param name="FolderName">Nombre de la subcarpeta.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo subdirectorio en Google Drive. Al terminar de crearse la carpeta, genera un Id de recurso de Drive.</remarks>
    Task CreateSubFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string FolderName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare);

    /// <summary>
    /// Función que crear una subcarpeta con permisos para un rol y/o grupo dentro de una carpeta.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive</param>
    /// <param name="FolderName">Nombre de la carpeta nueva.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo identificador del recurso en Google Drive en el recurso de origen de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task CreateSubFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que crear una subcarpeta con permisos para un rol y/o grupo dentro de una carpeta con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive</param>
    /// <param name="FolderName">Nombre de la carpeta nueva.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo identificador del recurso en Google Drive en el recurso de origen de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task CreateSubFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que crear una subcarpeta con permisos para un rol y/o grupo dentro de una carpeta con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive</param>
    /// <param name="FolderName">Nombre de la carpeta nueva.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <remarks>Genera un nuevo identificador del recurso en Google Drive en el recurso de origen de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task CreateSubFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare);

    /// <summary>
    /// Función que elimina un recurso de Google Drive, sea una carpeta o un archivo.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="SourceId">Identificador del recurso.</param>
    /// <remarks>Elimina el identificador del recurso en Google Drive. Si el recurso es una carpeta, se borrará todo el contenido que tiene la carpeta de Drive.</remarks>
    Task DeleteSourceAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string SourceId);

    /// <summary>
    /// Función que elimina un recurso de Google Drive, sea una carpeta o un archivo con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="SourceId">Identificador del recurso.</param>
    /// <remarks>Elimina el identificador del recurso en Google Drive. Si el recurso es una carpeta, se borrará todo el contenido que tiene la carpeta de Drive.</remarks>
    Task DeleteSourceAsync(string ClientId, string SecretId, string NombreAplicacion, string SourceId);

    /// <summary>
    /// Función que elimina un recurso de Google Drive, sea una carpeta o un archivo con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="SourceId">Identificador del recurso.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <remarks>Elimina el identificador del recurso en Google Drive. Si el recurso es una carpeta, se borrará todo el contenido que tiene la carpeta de Drive.</remarks>
    Task DeleteSourceAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string SourceId, GoogleAPIModeAccessServiceAccount TypeAccess);

    /// <summary>
    /// Función que sube un archivo a una carpeta raíz de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <remarks>Genera un nuevo archivo en la carpeta raíz de Google Drive. Al terminar de crearse el archivo, genera un Id de recurso de Drive.</remarks>
    Task FileUploadAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string strFileName, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    /// Función que sube un archivo a una carpeta raíz de Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <remarks>Genera un nuevo archivo en la carpeta raíz de Google Drive. Al terminar de crearse el archivo, genera un Id de recurso de Drive.</remarks>
    Task FileUploadAsync(string ClientId, string SecretId, string NombreAplicacion, string strFileName, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    /// Función que sube un archivo a una carpeta raíz de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <returns>Genera un nuevo archivo en la carpeta raíz de Google Drive. Al terminar de crearse el archivo, genera un Id de recurso de Drive.></returns>
    Task FileUploadAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string strFileName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="CarpetaTemporal">Carpeta temporal donde se guarda los archivos temporales de Google.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string strFileName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que sube un archivo a una carpeta de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <remarks>Genera un nuevo archivo en una carpeta de Google Drive. Al terminar de crearse el archivo, genera un Id de recurso de Drive.</remarks>
    Task FileUploadInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string strFileName, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    /// Función que sube un archivo a una carpeta de Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <remarks>Genera un nuevo archivo en una carpeta de Google Drive. Al terminar de crearse el archivo, genera un Id de recurso de Drive.</remarks>
    Task FileUploadInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string strFileName, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    ///  Función que sube un archivo a una carpeta de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <returns></returns>
    Task FileUploadInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string strFileName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare, bool IsCanCopyContent);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario a una carpeta.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en la carpeta o subcarpeta de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadInFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario a una carpeta con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en la carpeta o subcarpeta de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadInFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que sube un archivo a la carpeta ráiz de Google Drive con permisos y grupo de usuario a una carpeta con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo en la carpeta o subcarpeta de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task FileUploadInFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    #endregion

    #region "Google Sheets."

    /// <summary>
    /// Función que convierte un archivo de datos a Google Sheet y que se sube a Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string ArchivoOrigen);

    /// <summary>
    /// Función que convierte un archivo de datos a Google Sheet y que se sube a Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileAsync(string ClientId, string SecretId, string NombreAplicacion, string ArchivoOrigen);

    /// <summary>
    /// unción que convierte un archivo de datos a Google Sheet y que se sube a Google Drive con una cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string ArchivoOrigen, GoogleAPIModeAccessServiceAccount TypeAccess);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, desde la carpeta raíz de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Nombre del archivo de datos de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo de Google Sheets en la carpeta raíz de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task GenerateSheetFromDataFileWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, desde la carpeta raíz de Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Nombre del archivo de datos de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo de Google Sheets en la carpeta raíz de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task GenerateSheetFromDataFileWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, desde la carpeta raíz de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="ArchivoOrigen">Nombre del archivo de datos de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo de Google Sheets en la carpeta raíz de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task GenerateSheetFromDataFileWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que genera un sheet en un subdirectorio o directorio de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del folder o subcarpeta de Google Drive</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string ArchivoOrigen);

    /// <summary>
    /// Función que genera un sheet en un subdirectorio o directorio de Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del folder o subcarpeta de Google Drive</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string ArchivoOrigen);

    /// <summary>
    /// Función que genera un sheet en un subdirectorio o directorio de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del folder o subcarpeta de Google Drive</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string ArchivoOrigen, GoogleAPIModeAccessServiceAccount TypeAccess);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, en una carpeta existente de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="ArchivoOrigen">Nombre del archivo de datos de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo de Google Sheets en la carpeta raíz de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, en una carpeta existente de Google Drive con clave secreta.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de la carpeta de Google Drive.</param>
    /// <param name="ArchivoOrigen">Nombre del archivo de datos de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <remarks>Genera un nuevo identificador del archivo de Google Sheets en la carpeta raíz de Google Drive, asignando permisos de escritura o lectura y a un grupo o rol especifico.</remarks>
    Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    /// <summary>
    /// Función que genera un archivo de Google Sheets desde un archivo de datos y que se asigna a un usuario con un rol y grupo especifico, en una carpeta existente de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador del folder o subcarpeta de Google Drive</param>
    /// <param name="ArchivoOrigen">Archivo de origen.</param>
    /// <param name="EmailAddress">Dirección de correo electrónico de Gmail.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <param name="UserRole">Rol asignado.</param>
    /// <param name="Group">Grupo asignado.</param>
    /// <param name="IsCanShare">Indica si los editores pueden cambiar el acceso y añadir nuevos usuarios de Google.</param>
    /// <param name="IsCanCopyContent">Permitir si el archivo se puede modificar, copiar o descargar.</param>
    /// <param name="IsApplyCadicityNextDay">Indica si se aplica caducidad para un día limite.</param>
    /// <returns>Genera un nuevo archivo de Google Sheets con el contenido de origen.</returns>
    Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay);

    #endregion

    #region "Listado de archivos de Google Drive."

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string ArchivoCredencialesGoogle, string NombreAplicacion);

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string ClientId, string SecretId, string NombreAplicacion);

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, GoogleAPIModeAccessServiceAccount TypeAccess);

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive de una carpeta o subcarpeta de Google Drive.
    /// </summary>
    /// <param name="ArchivoCredencialesGoogle">Nombre del archivo de credenciales de Google API.</param>
    /// <param name="CarpetaTemporal">Carpeta temporal donde se guarda los archivos temporales de Google.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de una carpeta o subcarpeta de Google Drive.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId);

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive de una carpeta o subcarpeta de Google Drive con clave secreta del cliente.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de una carpeta o subcarpeta de Google Drive.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId);

    /// <summary>
    /// Función que genera una lista de archivos en Google Drive de una carpeta o subcarpeta de Google Drive con cuenta de servicio.
    /// </summary>
    /// <param name="AccountEmail">Correo o cuenta de servicio.</param>
    /// <param name="AccountKeyFile">Archivo de llaves o JSON que contiene la clave privada de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto de Google API.</param>
    /// <param name="FolderId">Identificador de una carpeta o subcarpeta de Google Drive.</param>
    /// <param name="TypeAccess">Modo de acceso: por JSON o clave privada.</param>
    /// <returns>Devuelve una lista generica del contenido del repositorio de Google Drive.</returns>
    Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, GoogleAPIModeAccessServiceAccount TypeAccess);

    #endregion
  }
}
