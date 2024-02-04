namespace RefreshNorthcrestDataFromFellowshipOne.Common.Constants
{
    public class ServiceConstants
    {
        // Local App Configurations
        public const string APICONFIG_REFRESH_PERSONNEL_DATA = "AppLocalConfig:RefreshPersonnelData";
        //public const string REFRESH_GENERAL_SONG_DATA = "AppLocalConfig:RefreshGeneralSongData";
        //public const string REFRESH_PLAN_SONG_DATA = "AppLocalConfig:RefreshPlanSongData";
        //public const string REFRESH_SERMON_DATA = "AppLocalConfig:RefreshSermonData";
        public const string APICONFIG_NUMBER_OF_DAYS_TO_REFRESH_PERSONNEL_DATA = "AppLocalConfig:NumberOfDaysToRefreshPersonnelData";
        //public const string NUMBER_OF_DAYS_TO_REFRESH_PLAN_SONG_DATA = "AppLocalConfig:NumberOfDaysToRefreshPlanSongData";
        //public const string NUMBER_OF_DAYS_TO_REFRESH_HIS_SERMON_DATA = "AppLocalConfig:NumberOfDaysToRefreshHistoricalSermonData";
        //public const string NUMBER_OF_DAYS_TO_REFRESH_FUTURE_DATA = "AppLocalConfig:NumberofDaysToRefreshFutureData";

        // Fellowship One API Configurations
        public const string APICONFIG_APP_ID = "FellowshipOne:appID";
        public const string APICONFIG_SECRET = "FellowshipOne:secret";
        public const string APICONFIG_TOKEN_TYPE = "FellowshipOne:TokenType";
        public const string APICONFIG_TOKEN_VALUE = "FellowshipOne:TokenValue";
        public const string APICONFIG_MEDIA_TYPE = "FellowshipOne:MediaType";


        // Email
        public const string EMAIL_FROM = "Email:FromEmail";
        public const string EMAIL_TO_MEDIA = "Email:ToMedia";
        public const string EMAIL_TO_LAURA = "Email:ToLaura";
        public const string EMAIL_TO_RICHIE = "Email:ToRichie";
        public const string EMAIL_SERVER_ADDRESS = "Email:ServerAddress";
        public const string EMAIL_SERVER_USERID = "Email:ServerUserId";
        public const string EMAIL_SERVER_PASSWORD = "Email:ServerPassword";
        public const string EMAIL_SUBJECT = "LOG RESULTS FOR: Refresh Northcrest Database from Fellowship One";
        public const string EMAIL_SERVER_PORT = "Email:ServerPort";
        public const string EMAIL_SERVER_SSL = "Email:ServerSsl";

        // Fellowship One URLs
        public const string URI_BASE_URL = "Uri:BaseUrl";
        public const string URI_PEOPLE_ALL_WITH_ADDRESSES_COMMUNICATIONS_ATTRIBUTES
            = "Uri:PeopleAllWithAddressesCommunicationsAttributes";
        public const string URI_REQUIREMENTS_SEARCH = "Uri:RequirementsSearch";
        public const string URI_PEOPLE_SEARCH = "Uri:PeopleSearch";
        public const string URI_HOUSEHOLDS_SEARCH = "Uri:HouseholdsSearch";
        public const string TEST_URI = "Uri:TestUri";
        //public const string SUNDAY_EVENING_PLANS_URL_CONFIG = "Url:SundayEveningPlans";
        //public const string SPECIAL_PLANS_URL_CONFIG = "Url:SpecialPlans";

        // Service Types 
        //public const string SUNDAY_MORNING_SERVICE = "Sunday Morning Service";
        //public const string SUNDAY_MORNING_SERVICES = "Sunday Morning Services";
        //public const string SUNDAY_EVENING_SERVICE = "Sunday Evening Service";
        //public const string SUNDAY_EVENING_SERVICES = "Sunday Evening Services";
        //public const string SPECIAL_SERVICE = "Special Service";
        //public const string SPECIAL_SERVICES = "Special Services";

        // Service Item Types in Planning Center
        //public const string SERMON = "sermon";
        //public const string SONG = "song";

        // Local Storage folders
        //public const string FILE_PATH_SONG_AUDIO = @"C:\ExternalDatabaseFiles\SongFiles\";
    }
}