namespace RefreshNorthcrestDataFromPlanningCenter.Common.Constants
{
    public class ServiceConstants
    {
        // Local App Configurations
        public const string NUMBER_OF_DAYS_TO_REFRESH = "AppLocalConfig:NumberOfDaysToRefresh";

        // Planning Center API Configurations
        public const string RATE_LIMIT_REQUEST = "PlanningCenter:RateLimit";
        public const string RATE_PERIOD_REQUEST = "PlanningCenter:RatePeriod";
        public const string APP_ID = "PlanningCenter:appID";
        public const string SECRET = "PlanningCenter:secret";
        public const string BASIC = "PlanningCenter:Basic";

        // Email
        public const string EMAIL_FROM = "Email:FromEmail";
        public const string EMAIL_TO_MEDIA = "Email:ToMedia";
        public const string EMAIL_TO_LAURA = "Email:ToLaura";
        public const string EMAIL_TO_RICHIE = "Email:ToRichie";
        public const string EMAIL_SERVER_ADDRESS = "Email:ServerAddress";
        public const string EMAIL_SERVER_USERID = "Email:ServerUserId";
        public const string EMAIL_SERVER_PASSWORD = "Email:ServerPassword";
        public const string EMAIL_SUBJECT = "LOG RESULTS FOR: Refresh Northcrest Database from Planning Center";
        public const string EMAIL_SERVER_PORT = "Email:ServerPort";
        public const string EMAIL_SERVER_SSL = "Email:ServerSsl";

        // Planning Center URLs
        public const string SUNDAY_MORNING_PLANS_URL_CONFIG = "Url:SundayMorningPlans";
        public const string SUNDAY_EVENING_PLANS_URL_CONFIG = "Url:SundayEveningPlans";
        public const string SPECIAL_PLANS_URL_CONFIG = "Url:SpecialPlans";

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