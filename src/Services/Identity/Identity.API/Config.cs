﻿using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> ApiResource =>
            new ApiResource[]
            {
                new ApiResource("catalog", "Catalog Service")
                {
                    Scopes = { "catalog" }
                },
                new ApiResource("shelves", "Shelves Service")
                {
                    Scopes = { "shelves" }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog"),
                new ApiScope("shelves")
            };

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new Client[]
            {
                new Client
                {
                    ClientId = "catalogswaggerui",
                    ClientName = "Catalog Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{clientsUrl["CatalogApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["CatalogApi"]}/swagger/" },
                    AllowedScopes = { "catalog" }
                },

                new Client
                {
                    ClientId = "shelvesswaggerui",
                    ClientName = "Shelves Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{clientsUrl["ShelvesApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["ShelvesApi"]}/swagger/" },
                    AllowedScopes = { "shelves" }
                },
            };
        }
    }
}