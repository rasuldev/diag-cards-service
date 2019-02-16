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
            [UserStatusEnum.Accepted] = "Принят",
            [UserStatusEnum.Rejected] = "Отклонен",
            [UserStatusEnum.Deleted] = "Удален"
        };

        private static readonly Dictionary<CardStatusEnum, string> CardStatusLabels =
            new Dictionary<CardStatusEnum, string>()
            {
                [CardStatusEnum.All] = "Выберите статус карты",
                [CardStatusEnum.Registered] = "Зарегистрированные",
                [CardStatusEnum.Unregistered] = "Незарегистрированные",
                [CardStatusEnum.Deleted] = "Удаленные",
            };

        private static readonly Dictionary<FuelTypes, string> FuelTypeLabels =
            new Dictionary<FuelTypes, string>()
            {
                [FuelTypes.Petrol] = "Бензин",
                [FuelTypes.Diesel] = "Дизельное топливо",
                [FuelTypes.CompressedGas] = "Сжатый газ",
                [FuelTypes.LiquefiedGas] = "Сжиженный газ",
                [FuelTypes.Other] = "Прочее"
            };

        private static readonly Dictionary<BrakeTypes, string> BrakeTypeLabels =
            new Dictionary<BrakeTypes, string>()
            {
                [BrakeTypes.Mechanic] = "Механический - в основном мотоциклы (кат. A)",
                [BrakeTypes.Hydraulic] = "Гидравлический - в основном мотоциклы и легковые автомобили (кат. A, B)",
                [BrakeTypes.Pneumatic] = "Пневматический - в основном крупнотоннажные грузовики, прицепы или автобусы (кат. C, D, E)",
                [BrakeTypes.Combined] = "Комбинированный - бывают разные, следует уточнять",
                
            };

        private static readonly Dictionary<VehicleCategory, string> VehicleCategoryLabels =
            new Dictionary<VehicleCategory, string>()
            {
                [VehicleCategory.B] = "B легковые"
            };

        private static readonly Dictionary<VehicleCategoryCommon,string> VehicleCategoryCommonLabels = new Dictionary<VehicleCategoryCommon, string>()
        {
            [VehicleCategoryCommon.M1] = "M1 (пасс., не более 8 пассажиров)",
            [VehicleCategoryCommon.N1] = "N1 (грузовые, не более 3,5 тонн)"
        };

        private static readonly Dictionary<DocumentTypes, string> DocumentTypeLabels =
            new Dictionary<DocumentTypes, string>()
            {
                [DocumentTypes.RegistrationCertificate] = "Свидетельство о регистрации транспортного средства",
                [DocumentTypes.VehiclePassport] = "Паспорт транспортного средства"
            };

        private static readonly Dictionary<CardTypes, string> CardTypeLabels =
            new Dictionary<CardTypes, string>()
            {
                [CardTypes.Common] = "Обычное ТС",
                //[CardTypes.DangerLoad] = "Опасный груз",
                //[CardTypes.RouteTranport] = "Маршрутные перевозки",
                [CardTypes.Taxi] = "Такси",
                //[CardTypes.TrainingDrive] = "Учебная езда"
            };

        public static string Label(this Enum e)
        {
            switch (e)
            {
                case UserStatusEnum val:
                    return UserStatusLabels[val];
                case CardStatusEnum val:
                    return CardStatusLabels[val];
                case FuelTypes val:
                    return FuelTypeLabels[val];
                case BrakeTypes val:
                    return BrakeTypeLabels[val];
                case DocumentTypes val:
                    return DocumentTypeLabels[val];
                case VehicleCategory val:
                    return VehicleCategoryLabels[val];
                case VehicleCategoryCommon val:
                    return VehicleCategoryCommonLabels[val];
                case CardTypes val:
                    return CardTypeLabels[val];
                default:
                    return e.ToString();
            }
        }
    }
}