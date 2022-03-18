// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            //resource_catalog izni catalog_fullpermission verdim
            new ApiResource("resource_catalog"){Scopes = {"catalog_fullpermission"}},
            new ApiResource("resource_photo_stock"){Scopes = {"photo_stock_fullpermission"}},
            new ApiResource("resource_basket"){Scopes = {"basket_fullpermission"}},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {//Kullanıcı ile ilgili işlemler
                       new IdentityResources.Email(),
                       
                       //Eğerki email şifre gönderdikten sonra bir token almak istiyorsak mutllaka Id göndermemiz lazım
                       new IdentityResources.OpenId(),

                       new IdentityResources.Profile(),
                       new IdentityResource(){Name = "roles",DisplayName = "Roles",Description = "Kullanıcı rolleri",UserClaims = new[]{"role"}}//Rolleride alıyorum


                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {//Eirişim izinleri Scopelar tanımlandı
                new ApiScope("catalog_fullpermission","Catalog API için ful erişim"),

                new ApiScope("photo_stock_fullpermission","Photo Stock API için ful erişim"),

                new ApiScope("basket_fullpermission","Basket API için ful erişim"),

                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)//IdentityServerApi
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    //Client bilgilerini giriyorum
                    ClientName = "Asp.Net Core MVC",
                    ClientId = "WebMvcClient",
                    ClientSecrets ={new Secret("secret".Sha256())}, // şifremiz olarak düşünebiliriz
                    AllowedGrantTypes = GrantTypes.ClientCredentials, //İzin tipim
                    AllowedScopes = { "catalog_fullpermission", "photo_stock_fullpermission",IdentityServerConstants.LocalApi.ScopeName }// izin verilen scopelar
                },
                new Client
                {
                //Client bilgilerini giriyorum
                ClientName = "Asp.Net Core MVC",
                ClientId = "WebMvcClientForUser",
                AllowOfflineAccess = true,
                ClientSecrets ={new Secret("secret".Sha256())}, // şifremiz olarak düşünebiliriz
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, //İzin tipim
                AllowedScopes ={"basket_fullpermission",
                    IdentityServerConstants.StandardScopes.Email,IdentityServerConstants.StandardScopes.OpenId,IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,IdentityServerConstants.LocalApi.ScopeName,"roles"},// izin verilen scopelar
                AccessTokenLifetime = 1*60*60,//AccesToken'nın ömrünü belirtiyorum. Saniye cinsinden 60*60 yparak 1 saat veriyorum
                RefreshTokenExpiration = TokenExpiration.Absolute,//Kesin bir tarih veriyorum mesela 60 gün sonra alanamasın
                AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                RefreshTokenUsage = TokenUsage.ReUse//refresh token bir kere mi kullanılsın arka arkaya kullanılsınmı . Tekrar kullanılabilir seçiyoruz

                //Yani kullanıcı 60 gün benim sayfama bir kere bile girmezse accesstoken zamanı dolmuş olacak
                }
            };
    }
}