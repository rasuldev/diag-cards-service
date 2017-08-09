namespace EaisApi
{
    public enum CheckResults
    {
        Success,// 0 - проверка прошла успешно
        CouponNotFound, // 1 - талон не найден (не используется)
        CouponAlreadyExists, // 2 - талон уже существует
        DuplicateToday // 3 - сегодня уже были сохранены результаты ТО на ТС с указанным VIN и Государственным регистрационным знаком
    }
}