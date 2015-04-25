// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorInterfaces
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    [ServiceContract]
    public interface ICalculator
    {
        [OperationContract]
        Task<double> AddAsync(double valueOne, double valueTwo);

        [OperationContract]
        Task<double> SubtractAsync(double valueOne, double valueTwo);
    }
}