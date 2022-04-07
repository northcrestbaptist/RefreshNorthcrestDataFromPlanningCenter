using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RefreshNorthcrestDataFromPlanningCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to begin the data retieval process.");
            Console.ReadLine();

            string planningCenterWebsite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans/2450442/items";

            using (var client = new HttpClient())
            {
                string appID = "ddb345abfa196418e8e7ff9a0fcafcb39363ca938320bffc435dc28506ed1bbf";
                string secret = "c3349f1648eef806903fb01e4515f3a93f5a9c8e73a979778e5b4dc48a6b0e52";
                var authenticationString = $"{appID}:{secret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                
                var responseTask = client.GetAsync(planningCenterWebsite);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success!");

                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var planningCenterResults = readTask.Result;

                    Console.WriteLine(planningCenterResults);
                }
                else
                {
                    Console.WriteLine("Failure!");
                }
            }
            Console.ReadLine();
        }
    }
}
