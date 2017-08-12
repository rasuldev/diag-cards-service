using EaisApi;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebUI.Services
{
    public class SessionStorage: IUserStorage
    {
        private const string UserDataKey = "UserData";
        private readonly ISession _session;

        public SessionStorage(ISession session)
        {
            _session = session;
        }

        public ApiUserData LoadData()
        {
            return JsonConvert.DeserializeObject<ApiUserData>(_session.GetString(UserDataKey));
        }

        public void SaveData(ApiUserData data)
        {
            _session.SetString(UserDataKey, JsonConvert.SerializeObject(data));
        }
    }
}