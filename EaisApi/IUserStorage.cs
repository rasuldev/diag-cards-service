using System.Threading.Tasks;

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
        Task<ApiUserData> LoadData();
        Task SaveData(ApiUserData data);
    }

    
}