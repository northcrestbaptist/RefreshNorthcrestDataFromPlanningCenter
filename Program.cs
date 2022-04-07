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
                string appID = "f1dd2c80d2d57e65ba2c282ca1fbc1af59d039cf40e4c41d06022890ba066340";
                string secret = "c1fc1ea67b68524b6124423ee95195f361ed51df226770f03bd75d0a7bbe488c";
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
