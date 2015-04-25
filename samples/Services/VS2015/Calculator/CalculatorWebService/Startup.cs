// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorWebService
{
    using System.Web.Http;
    using Owin;

    public class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "Add",
                routeTemplate: "Add/{left}+{right}",
                defaults: new {controller = "Default", action = "Get"},
                constraints: new {});

            UnityConfig.RegisterComponents(config);

            app.UseWebApi(config);
        }
    }
}