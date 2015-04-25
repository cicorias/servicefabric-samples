// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorWebClient
{
    using System;
    using System.Net.Http;
    using System.Threading;

    internal class Program
    {
        private static void Main(string[] args)
        {
            string baseAddress = "http://localhost:80/calculator";

            // Start OWIN host 
            HttpClient client = new HttpClient();
            Random r = new Random();
            for (;;)
            {
                int left = r.Next(100);
                int right = r.Next(100);
                string url = string.Format("{0}Add/{1}+{2}", baseAddress, left, right);

                Console.WriteLine("Request:");
                Console.WriteLine(url);
                HttpResponseMessage response = client.GetAsync(url).Result;
                Console.WriteLine("Response:");
                Console.WriteLine(response);

                Console.WriteLine();
                Console.WriteLine("{0} + {1} = {2}", left, right, response.Content.ReadAsStringAsync().Result);
                Console.WriteLine();

                Thread.Sleep(1000);
            }
        }
    }
}