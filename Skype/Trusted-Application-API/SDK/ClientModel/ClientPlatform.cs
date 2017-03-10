﻿using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SfB.PlatformService.SDK.Common;

namespace Microsoft.SfB.PlatformService.SDK.ClientModel
{
    public class ClientPlatform : IClientPlatform
    {
        #region Private fields

        private ClientPlatformSettings m_platformSettings;

        #endregion

        #region Public properties

        public Uri DiscoverUri
        {
            get {
                if (m_platformSettings.DiscoverUri != null)
                {
                    return m_platformSettings.DiscoverUri;
                }
                else if (this.IsSandBoxEnv)
                {
                    return Constants.PlatformDiscoverUri_SandBox;
                }
                else
                {
                    return Constants.PlatformDiscoverUri_Prod;
                }
            }
        }


        public Guid AADClientId
        {
            get { return m_platformSettings.AADClientId; }
        }

        public string AADClientSecret
        {
            get { return m_platformSettings.AADClientSecret; }
        }

        internal bool IsSandBoxEnv
        {
            get { return m_platformSettings.IsSandBoxEnv; }
        }

        /// <summary>
        /// Callback url where events related to a conversation will be delivered by SfB
        /// </summary>
        internal string CustomizedCallbackUrl
        {
            get { return m_platformSettings.CustomizedCallbackUrl; }
        }

        internal bool IsInternalPartner
        {
            get { return m_platformSettings.IsInternalPartner; }
        }


        public X509Certificate2 AADAppCertificate { get; private set; }

        #endregion

        #region Internal properties

        internal IRestfulClientFactory RestfulClientFactory { get; set; }

        #endregion

        #region Constructors

        public ClientPlatform(ClientPlatformSettings platformSettings, IPlatformServiceLogger logger)
        {
            if(platformSettings == null)
            {
                throw new ArgumentNullException(nameof(platformSettings));
            }

            if(logger == null)
            {
                 throw new ArgumentNullException(nameof(logger));
            }

            m_platformSettings = platformSettings;
            Logger.RegisterLogger(logger);
            if (!string.IsNullOrEmpty(platformSettings.AppTokenCertThumbprint))
            {
                AADAppCertificate = CertificateHelper.LookupCertificate(X509FindType.FindByThumbprint, platformSettings.AppTokenCertThumbprint, StoreName.My, StoreLocation.LocalMachine);
                if (AADAppCertificate == null)
                {
                    AADAppCertificate = CertificateHelper.LookupCertificate(X509FindType.FindByThumbprint, platformSettings.AppTokenCertThumbprint, StoreName.My, StoreLocation.CurrentUser);
                }
                if (AADAppCertificate == null)
                {
                    throw new ArgumentException($"Certificate with thumbprint {platformSettings.AppTokenCertThumbprint} not found in store");
                }
            }
            RestfulClientFactory = new RestfulClientFactory();
        }

        #endregion
    }
}

/// <summary>
/// We put all not official supported features (workarounds to help developers) in this namespace
/// </summary>
namespace Microsoft.SfB.PlatformService.SDK.ClientModel.Internal
{
    /// <summary>
    /// Internal extensions for <see cref="ClientPlatform"/>
    /// </summary>
    public static class ClientPlatformExtensions
    {
        public static string GetCustomizedCallbackUrl(this ClientPlatform This)
        {
            return This.CustomizedCallbackUrl;
        }

        public static bool GetIsSandboxEnv(this ClientPlatform This)
        {
            return This.IsSandBoxEnv;
        }

        public static bool GetIsInternalPartner(this ClientPlatform This)
        {
            return This.IsInternalPartner;
        }
    }
}