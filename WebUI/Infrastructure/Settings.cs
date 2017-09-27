using System;
using System.IO;

namespace WebUI.Infrastructure
{
    public class Settings
    {
        private readonly string _settingsPath;
        public Settings(string settingsPath)
        {
            _settingsPath = settingsPath;
        }

        public int GetDayLimit()
        {
            try
            {
                return int.Parse(File.ReadAllText(_settingsPath));
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public bool SetDayLimit(int limit)
        {
            try
            {
                File.WriteAllText(_settingsPath, limit.ToString());
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}