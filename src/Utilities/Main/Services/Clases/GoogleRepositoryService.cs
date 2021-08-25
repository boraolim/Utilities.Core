// strFileName: GoogleRepositoryService.
// Clase 'GoogleRepositoryService' para las operaciones de Google Drive y Google Sheets.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 6 de julio de 2019.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;

/* Librerías de Google Apis. */
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;

using Utilities.Resources;

namespace Utilities
{
  /// <summary>
  /// Clase 'ToolService' para las funciones especificas del programador.
  /// </summary>
  [Serializable]
  public class GoogleRepositoryService : IGoogleRepositoryService
  {
    /* Objetos no administrados (variables locales a nivel de la clase). */
    protected ushort _intNumberErr;
    protected string _strMessage;
    protected ResourceManager _resourceData;
    protected string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive, SheetsService.Scope.Spreadsheets };
    protected UserCredential _oCredential;

    /* Servicius internos de 'Utilities'. */
    protected IToolService _iToolService;
    protected IDictionaryCollectionService _iDictionaryCollectionService;

    /// <summary>
    /// Ubicación física de la carpeta de autenticación de Google.
    /// </summary>
    protected string _folderAuth { get; set; }

    /// <summary>
    /// Token de ubicación de Google API.
    /// </summary>
    protected string _ubicacionToken { get; set; }

    /// <summary>
    /// Clave del Id. Google asociada a la cuenta de Google.
    /// </summary>
    protected string _idKeyGoogleDrive { get; set; }

    /// <summary>
    /// Identificador del permiso ID de la cuenta de Google.
    /// </summary>
    protected string _permissionId { get; set; }

    /// <summary>
    /// Recurso de Google Drive.
    /// </summary>
    protected string _urlSourceDrive { get; set; }

    /// <summary>
    /// Servicio de Google Drive v3.
    /// </summary>
    protected Google.Apis.Drive.v3.DriveService _googleDriveRepositoryV3 { get; set; }

    /// <summary>
    /// Servicio de Google Drive v2.
    /// </summary>
    protected Google.Apis.Drive.v2.DriveService _googleDriveRepositoryV2 { get; set; }

    /// <summary>
    /// Servicio de Google Sheets v4.
    /// </summary>
    protected SheetsService _googleSheetRepositoryv4 { get; set; }

    #region "Atributos."

    /* Asignando atributos a la interfaz. */
    public string FolderAuth { get => _folderAuth; set => _folderAuth = value; }
    public string UbicacionToken { get => _ubicacionToken; set => _ubicacionToken = value; }
    public string IdKeyGoogleDrive { get => _idKeyGoogleDrive; set => _idKeyGoogleDrive = value; }
    public string PermissionsId { get => _permissionId; set => _permissionId = value; }
    public string URLSourceDrive { get => _urlSourceDrive; set => _urlSourceDrive = value; }
    public Google.Apis.Drive.v3.DriveService GoogleDriveRepositoryV3 { get => _googleDriveRepositoryV3; set => _googleDriveRepositoryV3 = value; }
    public Google.Apis.Drive.v2.DriveService GoogleDriveRepositoryV2 { get => _googleDriveRepositoryV2; set => _googleDriveRepositoryV2 = value; }
    public SheetsService GoogleSheetRepositoryv4 { get => _googleSheetRepositoryv4; set => _googleSheetRepositoryv4 = value; }

    #endregion

    /// <summary>
    /// Constructor de la clase 'GoogleRepositoryService'.
    /// </summary>
    /// <param name="dictionaryCollectionService">Objeto del tipo 'IDictionaryCollectionService'.</param>
    public GoogleRepositoryService(IDictionaryCollectionService dictionaryCollectionService, IToolService toolService)
    {
      _iDictionaryCollectionService = dictionaryCollectionService; _iToolService = toolService;
    }

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty;
      _resourceData = new ResourceManager(typeof(LanguageSource));
    }

    #region "Google Drive."

    public async Task CreateFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderName, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 4001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 4002;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4003;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4004;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedInfo"), DateTime.Now)}",
            WritersCanShare = IsCanShare,                            // Activa el flag de "Evitar que los editores cambien el acceso y añadan nuevos usuarios".          
            MimeType = "application/vnd.google-apps.folder"
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderName, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 4101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 4102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4104;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedInfo"), DateTime.Now)}",
            WritersCanShare = IsCanShare,                            // Activa el flag de "Evitar que los editores cambien el acceso y añadan nuevos usuarios".          
            MimeType = "application/vnd.google-apps.folder"
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4100;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 4201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 4202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 4203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 4204;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 4205;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 4206;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4207;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4208;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedInfo"), DateTime.Now)}",
            WritersCanShare = IsCanShare,                            // Activa el flag de "Evitar que los editores cambien el acceso y añadan nuevos usuarios".          
            MimeType = "application/vnd.google-apps.folder"
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4200;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 4301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 4302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4304;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 4305;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 4306;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            WritersCanShare = IsCanShare,                            // Activa el flag de "Evitar que los editores cambien el acceso y añadan nuevos usuarios".          
            MimeType = "application/vnd.google-apps.folder"
          };

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 4401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 4402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4403;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4404;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 4405;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 4406;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            WritersCanShare = IsCanShare,
            MimeType = "application/vnd.google-apps.folder"
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync(); IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4400;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 4501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 4502;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 4503;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 4504;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 4505;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 4506;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4507;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4508;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 4509;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 4510;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            WritersCanShare = IsCanShare,
            MimeType = "application/vnd.google-apps.folder"
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync(); IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4500;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string FolderName, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 4601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 4602;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4603;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 4604;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4605;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedInfo"), DateTime.Now)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4600;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string FolderName, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 4701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 4702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4703;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 4704;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4705;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedInfo"), DateTime.Now)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4700;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string FolderName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 4801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 4802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 4803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 4804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 4805;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 4806;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4807;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 4808;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4809;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedInfo"), DateTime.Now)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync();
          IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4800;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 4901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 4902;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 4903;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 4904;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 4905;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 4906;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 4907;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync(); IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 4900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 5001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 5002;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5003;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 5004;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 5005;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 5006;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 5007;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync(); IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5000;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task CreateSubFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string FolderName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 5101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 5102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 5103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 5104;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 5105;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 5106;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5107;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 5108;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(FolderName) | FolderName.Length == 0)
        {
          _intNumberErr = 5109;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleDriveFolderDestRequired")}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 5110;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 5111;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = FolderName,
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPISubFolderCreatedByInfo"), DateTime.Now, EmailAddress)}",
            MimeType = "application/vnd.google-apps.folder",
            WritersCanShare = IsCanShare,
            Parents = new List<string> { FolderId }
          };

          var request = GoogleDriveRepositoryV3.Files.Create(fileMetadata);
          request.Fields = "id, webViewLink"; var file = await request.ExecuteAsync(); IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;

          if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
          {
            var newPermission = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            newPermission = await GoogleDriveRepositoryV3.Permissions.Create(newPermission, IdKeyGoogleDrive).ExecuteAsync();
          }
          else if (UserRole == GoogleDrivePermissions.Owner)
          {
            var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
            {
              Type = Group.ToString().ToLower(),
              EmailAddress = EmailAddress,
              // ExpirationTimeRaw = Tool.ToRfc3339String(DateTime.Now.AddDays(1)),
              Role = UserRole.ToString().ToLower()
            };

            var _oCreateRequestPermission = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
            _oCreateRequestPermission.TransferOwnership = true;
            await _oCreateRequestPermission.ExecuteAsync();
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5100;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task DeleteSourceAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string SourceId)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 5201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 5202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          /* Realizo la eliminación del recurso de Google Drive. */
          var request = await GoogleDriveRepositoryV3.Files.Delete(SourceId).ExecuteAsync();
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5200;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task DeleteSourceAsync(string ClientId, string SecretId, string NombreAplicacion, string SourceId)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 5301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 5302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          /* Realizo la eliminación del recurso de Google Drive. */
          var request = await GoogleDriveRepositoryV3.Files.Delete(SourceId).ExecuteAsync();
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5300;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task DeleteSourceAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string SourceId, GoogleAPIModeAccessServiceAccount TypeAccess)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 5401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 5402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 5403;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 5404;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 5405;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 5406;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5407;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          /* Realizo la eliminación del recurso de Google Drive. */
          var request = await GoogleDriveRepositoryV3.Files.Delete(SourceId).ExecuteAsync();
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5400;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string strFileName, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 5501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 5502;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5503;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 5504;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 5505;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5500;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadAsync(string ClientId, string SecretId, string NombreAplicacion, string strFileName, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 5601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 5602;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5603;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 5604;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 5605;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5600;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string strFileName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 5701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 5702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 5703;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 5704;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 5705;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 5706;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5707;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 5708;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 5709;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5700;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 5801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 5802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 5804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 5805;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 5806;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 5807;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 5808;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 5901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 5902;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 5903;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 5904;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 5905;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 5906;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 5907;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 5908;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 5900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string strFileName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 6001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 6002;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 6003;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 6004;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 6005;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 6006;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6007;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6008;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6009;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 6010;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 6011;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del strFileName para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 6012;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string strFileName, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 6101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 6102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6104;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6105;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6106;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent,
            Parents = new List<string> { FolderId }
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6100;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string strFileName, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 6201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 6202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6204;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6205;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6206;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent,
            Parents = new List<string> { FolderId }
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6200;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string strFileName, GoogleAPIModeAccessServiceAccount TypeAccess, bool IsCanShare, bool IsCanCopyContent)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 6301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 6302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 6303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 6304;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 6305;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 6306;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6307;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6308;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6309;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6310;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          /* Realizamos la subida del archivo. */
          var FileMetaData = new Google.Apis.Drive.v3.Data.File()
          {
            Name = Path.GetFileName(strFileName),
            Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
            MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
            CopyRequiresWriterPermission = IsCanCopyContent,
            WritersCanShare = IsCanShare,
            ViewersCanCopyContent = IsCanCopyContent,
            Parents = new List<string> { FolderId }
          };

          using (var stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
          {
            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
            await request.UploadAsync(); var file = request.ResponseBody; IdKeyGoogleDrive = file.Id; URLSourceDrive = file.WebViewLink;
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6300;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 6401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 6402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6403;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6404;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6405;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6406;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 6407;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 6408;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 6409;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin de using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6400;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 6501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 6502;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6503;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6504;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6505;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6506;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 6507;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 6508;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 6509;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin de using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6500;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task FileUploadInFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string strFileName, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 6601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 6602;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 6603;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 6604;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 6605;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 6606;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6607;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 6608;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 6609;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 6610;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 6611;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 6612;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(strFileName);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(strFileName),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = await _iDictionaryCollectionService.GetMimeTypeAsync(Path.GetExtension(strFileName)),
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, FileMetaData.MimeType);

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 6613;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin de using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6600;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    #endregion

    #region "Google Sheets"

    public async Task GenerateSheetFromDataFileAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string ArchivoOrigen)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 6701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 6702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6703;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 6704;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 6705;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet"
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 6706;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6700;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileAsync(string ClientId, string SecretId, string NombreAplicacion, string ArchivoOrigen)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 6801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 6802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 6804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 6805;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet"
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 6806;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6800;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string ArchivoOrigen, GoogleAPIModeAccessServiceAccount TypeAccess)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 6901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 6902;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 6903;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 6904;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 6905;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 6906;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 6907;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 6908;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 6909;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet"
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 6910;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 6900;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 7001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 7002;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7003;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7004;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7005;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7006;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7007;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7008;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7000;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 7101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 7102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7104;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7105;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7106;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7107;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7108;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7100;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string ArchivoOrigen, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 7201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 7202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 7203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 7204;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 7205;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 7206;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7207;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7208;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7209;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7210;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7211;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7212;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {request.Upload().Exception.Message.Trim()}";
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7200;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string ArchivoOrigen)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 7301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 7302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7304;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7305;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7306;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 7307;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7300;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string ArchivoOrigen)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 7401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 7402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7403;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7404;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7405;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7406;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 7407;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7400;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string ArchivoOrigen, GoogleAPIModeAccessServiceAccount TypeAccess)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 7501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 7502;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 7503;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 7504;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 7205;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 7506;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7507;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7508;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7509;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7510;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedInfo"), DateTime.Now)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;
            }
            else
            {
              _intNumberErr = 7511;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using.
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7500;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 7601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 7602;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7603;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7604;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7605;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7606;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7607;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7608;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7609;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7600;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 7701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 7702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7703;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7704;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7705;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7706;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7707;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7708;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7709;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7700;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task GenerateSheetFromDataFileInFolderWithPermissionAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, string ArchivoOrigen, string EmailAddress, GoogleAPIModeAccessServiceAccount TypeAccess, GoogleDrivePermissions UserRole, GoogleDriveGroups Group, bool IsCanShare, bool IsCanCopyContent, bool IsApplyCadicityNextDay)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 7801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 7802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 7803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 7804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 7805;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 7806;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7807;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 7808;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else if (string.IsNullOrEmpty(ArchivoOrigen) | ArchivoOrigen.Length == 0)
        {
          _intNumberErr = 7809;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(ArchivoOrigen))
        {
          _intNumberErr = 7810;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), ArchivoOrigen)}";
        }
        else if (string.IsNullOrEmpty(EmailAddress) | EmailAddress.Length == 0)
        {
          _intNumberErr = 7811;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(EmailAddress, Patterns.EmailPattern))
        {
          _intNumberErr = 7812;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          byte[] byteArray = await File.ReadAllBytesAsync(ArchivoOrigen);
          using (var stream = new MemoryStream(byteArray))
          {
            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
              Name = Path.GetFileName(ArchivoOrigen),
              Description = $"{string.Format(_resourceData.GetString("strGoogleAPIFileCreatedByInfo"), DateTime.Now, EmailAddress)}",
              MimeType = "application/vnd.google-apps.spreadsheet",
              CopyRequiresWriterPermission = IsCanCopyContent,
              WritersCanShare = IsCanShare,
              ViewersCanCopyContent = IsCanCopyContent,
              Parents = new List<string> { FolderId }
            };

            var request = GoogleDriveRepositoryV3.Files.Create(FileMetaData, stream, "text/csv"); request.Fields = "id, webViewLink";

            if (request.Upload().Exception == null)
            {
              IdKeyGoogleDrive = request.ResponseBody.Id; URLSourceDrive = request.ResponseBody.WebViewLink;

              if (UserRole == GoogleDrivePermissions.Reader || UserRole == GoogleDrivePermissions.Writter)
              {
                var newPermissionUser = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToUser = GoogleDriveRepositoryV3.Permissions.Create(newPermissionUser, IdKeyGoogleDrive);
                _oCreateRequestPermissionToUser.Fields = "id"; await _oCreateRequestPermissionToUser.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToUser = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermission = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToUser, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermission.Fields = "id, expirationTime"; await _oUpdateRequestPermission.ExecuteAsync();
                }
              }
              else if (UserRole == GoogleDrivePermissions.Owner)
              {
                var newPermissionOwner = new Google.Apis.Drive.v3.Data.Permission
                {
                  Type = Group.ToString().ToLower(),
                  EmailAddress = EmailAddress,
                  Role = UserRole.ToString().ToLower()
                };

                var _oCreateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Create(newPermissionOwner, IdKeyGoogleDrive);
                _oCreateRequestPermissionToOwner.TransferOwnership = true; _oCreateRequestPermissionToOwner.Fields = "id";
                await _oCreateRequestPermissionToOwner.ExecuteAsync();

                // Obtengo el identificador del permiso del archivo para el correo electrónico en especifico.
                PermissionsId = GetPermissionIdForEmail(GoogleDriveRepositoryV3, EmailAddress);

                if (IsApplyCadicityNextDay)
                {
                  // Aplicamos la caducidad.
                  var _oUpdateExpiryPermissionToOwner = new Google.Apis.Drive.v3.Data.Permission
                  {
                    Role = UserRole.ToString().ToLower(),
                    ExpirationTimeRaw = _iToolService.ToRfc3339String(DateTime.Now.AddDays(1))
                  };

                  // Actualizamos la fecha de expiración.
                  var _oUpdateRequestPermissionToOwner = GoogleDriveRepositoryV3.Permissions.Update(_oUpdateExpiryPermissionToOwner, IdKeyGoogleDrive, PermissionsId);
                  _oUpdateRequestPermissionToOwner.Fields = "id, expirationTime"; await _oUpdateRequestPermissionToOwner.ExecuteAsync();
                }
              }
            }
            else
            {
              _intNumberErr = 7813;
              _strMessage = string.Concat("Ocurrió un error: ", request.Upload().Exception.Message.Trim());
            }
          } // Fin del using
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7800;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    #endregion

    #region "Listado de archivos de Google Drive."

    public async Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string ArchivoCredencialesGoogle, string NombreAplicacion)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 7901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 7902;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 7903;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);

          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents)";

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 7900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    public async Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string ClientId, string SecretId, string NombreAplicacion)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 8001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 8002;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 8003;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents)";

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    public async Task<List<GoogleDriveFiles>> GetDriveFilesAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, GoogleAPIModeAccessServiceAccount TypeAccess)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 8101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 8102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 8103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 8104;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 8105;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 8106;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 8107;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents)";

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8100;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    public async Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string ArchivoCredencialesGoogle, string NombreAplicacion, string FolderId)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(ArchivoCredencialesGoogle) | ArchivoCredencialesGoogle.Length == 0)
        {
          _intNumberErr = 8201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStreamRequired")}";
        }
        else if (!File.Exists(ArchivoCredencialesGoogle))
        {
          _intNumberErr = 8202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleFileStremNotExists")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 8203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 8204;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ArchivoCredencialesGoogle, NombreAplicacion);
          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents, permissions, viewersCanCopyContent)";
          FileListRequest.Key = FolderId;

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents,
                Permissions = file.Permissions,
                ViewersCanCopyContent = file.ViewersCanCopyContent
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8200;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    public async Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string ClientId, string SecretId, string NombreAplicacion, string FolderId)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(ClientId) | ClientId.Length == 0)
        {
          _intNumberErr = 8301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleClientId")}";
        }
        else if (string.IsNullOrEmpty(SecretId) | SecretId.Length == 0)
        {
          _intNumberErr = 8302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleSecretId")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 8303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 8304;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(ClientId, SecretId, NombreAplicacion);

          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents, permissions, viewersCanCopyContent)";
          FileListRequest.Key = FolderId;

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents,
                Permissions = file.Permissions,
                ViewersCanCopyContent = file.ViewersCanCopyContent
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8300;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    public async Task<List<GoogleDriveFiles>> GetDriveFilesInFolderAsync(string AccountEmail, string AccountKeyFile, string NombreAplicacion, string FolderId, GoogleAPIModeAccessServiceAccount TypeAccess)
    {
      InitVars(); var _oFileList = new List<GoogleDriveFiles>();

      try
      {
        if (string.IsNullOrEmpty(AccountEmail) | AccountEmail.Length == 0)
        {
          _intNumberErr = 8401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountRequired")}";
        }
        else if (!Regex.IsMatch(AccountEmail, Patterns.EmailPattern))
        {
          _intNumberErr = 8402;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleServiceAccountNotValid")}";
        }
        else if (string.IsNullOrEmpty(AccountKeyFile) | AccountKeyFile.Length == 0)
        {
          _intNumberErr = 8403;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileKeyRequired")}";
        }
        else if (!File.Exists(AccountKeyFile))
        {
          _intNumberErr = 8404;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), AccountKeyFile)}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByJSONFile && !Regex.IsMatch(AccountKeyFile, Patterns.JSONExtensionPattern))
        {
          _intNumberErr = 8405;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionJSONInvalid")}";
        }
        else if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile && !Regex.IsMatch(AccountKeyFile, Patterns.KeyExtensionPattern))
        {
          _intNumberErr = 8406;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileExtensionKeyInvalid")}";
        }
        else if (string.IsNullOrEmpty(NombreAplicacion) | NombreAplicacion.Length == 0)
        {
          _intNumberErr = 8407;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleApplicationNameRequired")}";
        }
        else if (string.IsNullOrEmpty(FolderId) | FolderId.Length == 0)
        {
          _intNumberErr = 8408;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGoogleAPISourceId")}";
        }
        else
        {
          /* Inicializo los servicios. */
          GenerateDriveService(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);

          Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest;

          FileListRequest = GoogleDriveRepositoryV3.Files.List();
          FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, md5Checksum, mimeType, size, version, trashed, owners, webViewLink, parents, permissions, viewersCanCopyContent)";
          FileListRequest.Key = FolderId;

          // Listamos el contenido.
          var _oFiles = await FileListRequest.ExecuteAsync();

          if (_oFiles.Files != null && _oFiles.Files.Count > 0)
          {
            foreach (var file in _oFiles.Files)
            {
              GoogleDriveFiles File = new GoogleDriveFiles
              {
                Id = file.Id,
                Name = file.Name,
                Size = file.Size,
                MD5Checksum = file.Md5Checksum,
                MimeType = file.MimeType,
                Version = file.Version,
                Owners = file.Owners,
                webViewLink = file.WebViewLink,
                CreatedTime = file.CreatedTime,
                Parents = file.Parents,
                Permissions = file.Permissions,
                ViewersCanCopyContent = file.ViewersCanCopyContent
              };
              _oFileList.Add(File);
            } // Fin del bucle foreach.
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8400;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _oFileList;
    }

    #endregion

    #region "Funciones privadas."

    /// <summary>
    /// Función privada de generación del servicio de Google Drive (v3) por medio del strFileName de credenciales de Google API.
    /// </summary>
    /// <param name="ArchivoCredenciales">strFileName de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v3).</returns>
    private Google.Apis.Drive.v3.DriveService GetService_v3(string ArchivoCredenciales, string NombreAplicacion)
    {
      var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
      if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

      using (var stream = new FileStream(ArchivoCredenciales, FileMode.Open, FileAccess.Read))
      {
        _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;

        // Guardo la ubicación del token generado.
        UbicacionToken = _credPath;
      }

      //Create Drive API service.
      var service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Drive (v3) por medio de la clave del cliente y su clave secreta.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v3).</returns>
    private Google.Apis.Drive.v3.DriveService GetService_v3(string ClientId, string SecretId, string NombreAplicacion)
    {
      var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
      if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

      _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = ClientId, ClientSecret = SecretId }, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;
      // Guardo la ubicación del token generado.
      UbicacionToken = _credPath;

      //Create Drive API service.
      var service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Drive (v3) por medio de una cuenta de servicio.
    /// </summary>
    /// <param name="TypeAccess">Tipo de acceso: por JSON o llave privada.</param>
    /// <param name="AccountEmail">Cuenta de servicio de Google.</param>
    /// <param name="AccountKeyFile">strFileName de autenticación de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v3).</returns>
    private Google.Apis.Drive.v3.DriveService GetService_v3(GoogleAPIModeAccessServiceAccount TypeAccess, string AccountEmail, string AccountKeyFile, string NombreAplicacion)
    {
      ServiceAccountCredential _oSvcCredential = null;

      if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile)
      {
        var certificate = new X509Certificate2(AccountKeyFile, "notasecret", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
        _oSvcCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(AccountEmail) { Scopes = Scopes }.FromCertificate(certificate));
      }
      else
      {
        var json = File.ReadAllText(AccountKeyFile);
        var cr = JsonConvert.DeserializeObject<PersonalServiceAccountCredential>(json);
        _oSvcCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.client_email) { Scopes = Scopes }.FromPrivateKey(cr.private_key));
      }

      /* Create Drive API service. */
      var service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oSvcCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Drive (v2) por medio del strFileName de credenciales de Google API.
    /// </summary>
    /// <param name="ArchivoCredenciales">strFileName de credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v2).</returns>
    private Google.Apis.Drive.v2.DriveService GetService_v2(string ArchivoCredenciales, string NombreAplicacion)
    {
      var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
      if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

      using (var stream = new FileStream(ArchivoCredenciales, FileMode.Open, FileAccess.Read))
      {
        _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;
        // Guardo la ubicación del token generado.
        UbicacionToken = _credPath;
      }

      //Create Drive API service.
      var service = new Google.Apis.Drive.v2.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de eneración del servicio de Google Drive (v2) por medio de la clave del cliente y su clave secreta.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v2).</returns>
    private Google.Apis.Drive.v2.DriveService GetService_v2(string ClientId, string SecretId, string NombreAplicacion)
    {
      var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
      if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

      _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = ClientId, ClientSecret = SecretId }, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;
      // Guardo la ubicación del token generado.
      UbicacionToken = _credPath;


      //Create Drive API service.
      var service = new Google.Apis.Drive.v2.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Drive (v2) por medio de una cuenta de servicio.
    /// </summary>
    /// <param name="TypeAccess">Tipo de acceso: por JSON o llave privada.</param>
    /// <param name="AccountEmail">Cuenta de servicio de Google.</param>
    /// <param name="AccountKeyFile">strFileName de autenticación de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v3).</returns>
    private Google.Apis.Drive.v2.DriveService GetService_v2(GoogleAPIModeAccessServiceAccount TypeAccess, string AccountEmail, string AccountKeyFile, string NombreAplicacion)
    {
      ServiceAccountCredential _oSvcCredential = null;

      if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile)
      {
        var certificate = new X509Certificate2(AccountKeyFile, "notasecret", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
        _oSvcCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(AccountEmail) { Scopes = Scopes }.FromCertificate(certificate));
      }
      else
      {
        var json = File.ReadAllText(AccountKeyFile);
        var cr = JsonConvert.DeserializeObject<PersonalServiceAccountCredential>(json);
        _oSvcCredential= new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.client_email) { Scopes = Scopes }.FromPrivateKey(cr.private_key));
      }

      /* Create Drive API service. */
      var service = new Google.Apis.Drive.v2.DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oSvcCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Sheets (v4) por medio del strFileName de credenciales de Google API.
    /// </summary>
    /// <param name="ArchivoCredenciales">strFileName credenciales de Google API.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto SheetService.</returns>
    private SheetsService GetSheetService_v4(string ArchivoCredenciales, string NombreAplicacion)
    {
      using (var stream = new FileStream(ArchivoCredenciales, FileMode.Open, FileAccess.Read))
      {
        var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
        if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

        _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;
        
        // Guardo la ubicación del token generado.
        UbicacionToken = _credPath;
      }

      //Create Drive API service.
      var service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función de generación del servicio de Google Sheets por medio de la clave del cliente y su clave secreta.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto SheetService.</returns>
    private SheetsService GetSheetService_v4(string ClientId, string SecretId, string NombreAplicacion)
    {
      var _credPath = Path.Combine(_folderAuth, Assembly.GetExecutingAssembly().GetName().Name);
      if (!Directory.Exists(_credPath)) Directory.CreateDirectory(_credPath);

      _oCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = ClientId, ClientSecret = SecretId }, Scopes, "user", CancellationToken.None, new FileDataStore(_credPath, true)).Result;

      // Guardo la ubicación del token generado.
      UbicacionToken = _credPath;

      //Create Drive API service.
      var service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = _oCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Funcion de generación del servicio de Google Sheets por medio por medio de una cuenta de servicio.
    /// </summary>
    /// <param name="TypeAccess">Tipo de acceso: por JSON o llave privada.</param>
    /// <param name="AccountEmail">Cuenta de servicio de Google.</param>
    /// <param name="AccountKeyFile">strFileName de autenticación de la cuenta de servicio.</param>
    /// <param name="NombreAplicacion">Nombre del proyecto o aplicación de Google API.</param>
    /// <returns>Si la autenticación es satisfactoria, se generará un objeto DriveService (v3).</returns>
    private SheetsService GetSheetService_v4(GoogleAPIModeAccessServiceAccount TypeAccess, string AccountEmail, string AccountKeyFile, string NombreAplicacion)
    {
      ServiceAccountCredential _oSvcCredential = null;

      if (TypeAccess == GoogleAPIModeAccessServiceAccount.ByKeyFile)
      {
        var certificate = new X509Certificate2(AccountKeyFile, "notasecret", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
        _oSvcCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(AccountEmail) { Scopes = Scopes }.FromCertificate(certificate));
      }
      else
      {
        var json = File.ReadAllText(AccountKeyFile);
        var cr = JsonConvert.DeserializeObject<PersonalServiceAccountCredential>(json);
        _oSvcCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.client_email) { Scopes = Scopes }.FromPrivateKey(cr.private_key));
      }

      //Create Drive API service.
      var service = new SheetsService(new BaseClientService.Initializer() 
      {
        HttpClientInitializer = _oSvcCredential,
        ApplicationName = NombreAplicacion,
      });

      return service;
    }

    /// <summary>
    /// Función privada que inicializa los servicios de las API's de Google.
    /// </summary>
    /// <param name="ArchivoCredenciales">strFileName de credenciales de Google API.</param>
    /// <param name="CarpetaTemporalPersonal">Carpeta personal temporal donde se guarda la clave de autenticación.</param>
    /// <param name="NombreAplicacion">Nombre de la aplicación de Google API.</param>
    private void GenerateDriveService(string ArchivoCredenciales, string NombreAplicacion)
    {
      _googleDriveRepositoryV2 = GetService_v2(ArchivoCredenciales, NombreAplicacion);                // Inicializa el servicio 2 de Drive.
      _googleDriveRepositoryV3 = GetService_v3(ArchivoCredenciales, NombreAplicacion);                // Inicializa el servicio 3 de Drive.
      _googleSheetRepositoryv4 = GetSheetService_v4(ArchivoCredenciales, NombreAplicacion);           // Inicializa el servicio de Sheets.
    }

    /// <summary>
    /// Función privada que inicializa los servicios de las API's de Google.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre de la aplicación de Google API.</param>
    private void GenerateDriveService(string ClientId, string SecretId, string NombreAplicacion)
    {
      _googleDriveRepositoryV2 = GetService_v2(ClientId, SecretId, NombreAplicacion);                 // Inicializa el servicio 2 de Drive.
      _googleDriveRepositoryV3 = GetService_v3(ClientId, SecretId, NombreAplicacion);                 // Inicializa el servicio 3 de Drive.
      _googleSheetRepositoryv4 = GetSheetService_v4(ClientId, SecretId, NombreAplicacion);            // Inicializa el servicio de Sheets.
    }

    /// <summary>
    /// Función privada que inicializa los servicios de las API's de Google.
    /// </summary>
    /// <param name="ClientId">Clave del cliente.</param>
    /// <param name="SecretId">Clave secreta del cliente.</param>
    /// <param name="NombreAplicacion">Nombre de la aplicación de Google API.</param>
    private void GenerateDriveService(GoogleAPIModeAccessServiceAccount TypeAccess, string AccountEmail, string AccountKeyFile, string NombreAplicacion)
    {
      _googleDriveRepositoryV2 = GetService_v2(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);                 // Inicializa el servicio 2 de Drive.
      _googleDriveRepositoryV3 = GetService_v3(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);                 // Inicializa el servicio 3 de Drive.
      _googleSheetRepositoryv4 = GetSheetService_v4(TypeAccess, AccountEmail, AccountKeyFile, NombreAplicacion);            // Inicializa el servicio de Sheets.
    }

    /// <summary>
    /// Función que obtiene el identificador del permiso del recurso de Google Drive.
    /// </summary>
    /// <param name="service">Servicio de Google API Drive v3.</param>
    /// <param name="emailAddress">Correo electrónico de destino.</param>
    /// <returns>Genera una cadena de texto de identificación del permiso para el correo destino de Google Drive.</returns>
    private string GetPermissionIdForEmail(Google.Apis.Drive.v3.DriveService service, string emailAddress)
    {
      string pageToken = null;

      do
      {
        var request = service.Files.List();
        request.Q = $"'{emailAddress}' in writers or '{emailAddress}' in readers or '{emailAddress}' in owners";
        request.Spaces = "drive";
        request.Fields = "nextPageToken, files(id, name, permissions)";
        request.PageToken = pageToken;

        var result = request.Execute();

        foreach (var file in result.Files.Where(f => f.Permissions != null))
        {
          var permission = file.Permissions.SingleOrDefault(p => string.Equals(p.EmailAddress, emailAddress, StringComparison.InvariantCultureIgnoreCase));

          if (permission != null)
            return permission.Id;
        }

        pageToken = result.NextPageToken;

      } while (pageToken != null);

      return null;
    }

    #endregion
  }
}
