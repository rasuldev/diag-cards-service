namespace EaisApi
{
    public class ApiUserData
    {
        public string Cookies { get; set; }
        public string CaptchaId { get; set; }

        public ApiUserData(string cookies, string captchaId)
        {
            Cookies = cookies;
            CaptchaId = captchaId;
        }
    }
    public interface IUserStorage
    {
        ApiUserData LoadData();
        void SaveData(ApiUserData data);
    }

    
}