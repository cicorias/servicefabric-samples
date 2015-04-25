// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace WebService
{
    using System.Web.Http;

    public static class RouteConfig
    {
        /// <summary>
        ///     Routing registration.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                "Default",
                "api/{controller}/{id}",
                new {controller = "Default", id = RouteParameter.Optional},
                new {}
                );
        }
    }
}