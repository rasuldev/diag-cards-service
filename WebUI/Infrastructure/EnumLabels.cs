using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using EaisApi.Models;
using WebUI.Models;

namespace WebUI.Infrastructure
{
    public static class EnumLabels
    {
        private static readonly Dictionary<UserStatusEnum, string> UserStatusLabels = new Dictionary<UserStatusEnum, string>()
        {
            [UserStatusEnum.All] = "Все",
            [UserStatusEnum.Waiting] = "Ожидающие подтверждения",
            [UserStatusEnum.Accepted] = "Принятые",
            [UserStatusEnum.Rejected] = "Отклоненные",
            [UserStatusEnum.Deleted] = "Удаленные"
        };

        private static readonly Dictionary<CardStatusEnum, string> CardStatusLabels =
            new Dictionary<CardStatusEnum, string>()
            {
                [CardStatusEnum.All] = "Выберите статус карты",
                [CardStatusEnum.Registered] = "Зарегистрированные",
                [CardStatusEnum.Unregistered] = "Незарегистрированные",
            };
        public static string Label(this Enum e)
        {
            switch (e)
            {
                case UserStatusEnum val:
                    return UserStatusLabels[val];
                case CardStatusEnum val:
                    return CardStatusLabels[val];
                default:
                    return e.ToString();
            }
        }
    }
}