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
            var data = _session.GetString(UserDataKey);
            return data == null ? null : JsonConvert.DeserializeObject<ApiUserData>(data);
        }

        public void SaveData(ApiUserData data)
        {
            _session.SetString(UserDataKey, JsonConvert.SerializeObject(data));
        }
    }
}