using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Common.Constants
{
    public class ServiceConstants
    {
        // Planning Center API Configurations
        public const string RATE_LIMIT_REQUEST = "X-PCO-API-Request-Rate-Limit";
        public const string RATE_PERIOD_REQUEST = "X-PCO-API-Request-Rate-Period";
        public const string APP_ID = "appID";
        public const string SECRET = "secret";
        public const string BASIC = "Basic";

        // Planning Center URLs
        public const string SUNDAY_MORNING_SERVICE_PLANS_URL = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans";
        public const string SUNDAY_EVENING_SERVICE_PLANS_URL = "https://api.planningcenteronline.com/services/v2/service_types/107396/plans";
        public const string SPECIAL_SERVICE_PLANS_URL = "https://api.planningcenteronline.com/services/v2/service_types/107397/plans";

        // Service Types 
        public const string SUNDAY_MORNING_SERVICE = "Sunday Morning Service";
        public const string SUNDAY_MORNING_SERVICES = "Sunday Morning Services";
        public const string SUNDAY_EVENING_SERVICE = "Sunday Evening Service";
        public const string SUNDAY_EVENING_SERVICES = "Sunday Evening Services";
        public const string SPECIAL_SERVICE = "Special Service";
        public const string SPECIAL_SERVICES = "Special Services";

        public const string SERMON = "sermon";
    }
}