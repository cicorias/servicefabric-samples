// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorWebService.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using CalculatorInterfaces;

    public class DefaultController : ApiController
    {
        private readonly ICalculator calculator;

        public DefaultController(ICalculator calculator)
        {
            this.calculator = calculator;
        }

        public async Task<double> Get(double left, double right)
        {
            return await this.calculator.AddAsync(left, right);
        }
    }
}