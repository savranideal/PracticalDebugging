﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalDebuggingWeb.Services
{
    internal class CouponService
    {
        internal static double GetCouponDiscount(string couponCode)
        {
            if (couponCode == "Employee2423432")
            {
                return 25;
            }
            else if (couponCode == "VIP_2020_rmg")
            {
                return 15;
            }
            else
            {
                AutoResetEvent are = new AutoResetEvent(false);
                double result = 0;

                ThreadPool.QueueUserWorkItem((_) =>
                {
                    result = GetCouponDiscountInternal(couponCode, are);
                });
                are.WaitOne();
                return result;
            }
        }

        private static double GetCouponDiscountInternal(string couponCode, AutoResetEvent are)
        {
            double result;
            var someValue = 10;
            var httpClient = HttpClientFactory.Create();
            var res = httpClient.GetAsync("https://couponservice.com/api/Discount?couponCode=" + couponCode)
                                .GetAwaiter().GetResult();
            var content = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            result = double.Parse(content);
            are.Set();
            return result;
        }
    }
}