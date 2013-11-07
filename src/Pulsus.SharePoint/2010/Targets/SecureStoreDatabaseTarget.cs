using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus.SharePoint.Targets
{
    public class SecureStoreDatabaseTarget : DatabaseTarget
    {
        public string AppId { get; set; }

        public static SPServiceContext Context
        {
            get
            {
                return SPServiceContext.GetContext(SPServiceApplicationProxyGroup.Default, SPSiteSubscriptionIdentifier.Default);
            }
        }

        protected override ConnectionSettings GetConnectionSettings()
        {
            if (string.IsNullOrEmpty(AppId))
            {
                PulsusDebugger.Error("The provided AppId is not valid. Please check configuration");
                return null;
            }

            var provider = SecureStoreProviderFactory.Create();
            ((ISecureStoreServiceContext)provider).Context = Context;

            using (var credentials = provider.GetCredentials(AppId))
            {
                if (credentials == null)
                    throw new Exception(string.Format("Unable to retrive the credentials for AppId '{0}'", AppId));

                // used to validate connection string
                var connectionStringBuilder = new DbConnectionStringBuilder();

                foreach (SecureStoreCredential credential in credentials)
                {
                    // as we don't have access to the credential name we can only evaluate the value
                    // to find a connection string

                    if (credential.CredentialType == SecureStoreCredentialType.Generic)
                    {
                        var connectionString = ReadSecureString(credential.Credential);

                        try
                        {
                            connectionStringBuilder.ConnectionString = connectionString;
                        }
                        catch (ArgumentException)
                        {
                            // this is not a connection string so move on
                            continue;
                        }
                        
                        return  new ConnectionSettings(connectionString);
                    }
                }
            }

            PulsusDebugger.Error("Unable to find a connection string in AppId '{0}' credentials. Please check the secure store", AppId);
            return null;
        }

        protected string ReadSecureString(SecureString secureString)
        {
            if (secureString == null)
                return null;

            var ptr = Marshal.SecureStringToBSTR(secureString);
            var str = Marshal.PtrToStringBSTR(ptr);
            Marshal.ZeroFreeBSTR(ptr);
            return str;
        }
    }
}
