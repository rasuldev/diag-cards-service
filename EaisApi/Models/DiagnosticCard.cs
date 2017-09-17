using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EaisApi.Models
{
    public class DiagnosticCard
    {
        [MaxLength(30)]
        public string CardId { get; set; }
        [Required(ErrorMessage = "Required"), Display(Name = "Фамилия"), MaxLength(30)]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Required"), Display(Name = "Имя"), MaxLength(30)]
        public string Firstname { get; set; }
        [Display(Name = "Отчество"), MaxLength(30)]
        public string Patronymic { get; set; }
        [StringLength(17), Display(Name = "VIN")]
        [RegularExpression("[A-Za-zА-Яа-яЁё0-9]{0,17}", ErrorMessage = "Данное поле может содержать только русские (А-Я), латинские (A-Z) буквы и цифры (0-9)")]
        public string VIN { get; set; }
        [Required(ErrorMessage = "Required"), Display(Name = "Год выпуска ТС")]
        [RegularExpression("\\d{4,4}", ErrorMessage = "Должно быть ровно 4 цифры")]
        [DataType(DataType.Text)]
        public int IssueYear { get; set; }

        /// <summary>
        /// Марка ТС
        /// </summary>
        [Required(ErrorMessage = "Required"), Display(Name = "Марка ТС"), MaxLength(30)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Модель ТС
        /// </summary>
        [Required(ErrorMessage = "Required"), Display(Name = "Модель ТС"), MaxLength(30)]
        public string Model { get; set; }

        [Display(Name = "Кузов №"), MaxLength(50)]
        public string BodyNumber { get; set; }
        [Display(Name = "Рама (Шасси) №"), MaxLength(50)]
        public string FrameNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Пробег ТС (км)")]
        [Range(100, int.MaxValue, ErrorMessage = "Должно быть указано целое число, не меньшее {1}")]
        [RegularExpression("\\d+", ErrorMessage = "Должно быть указано целое число")]
        [DataType(DataType.Text)]
        public int Running { get; set; }
        /// <summary>
        /// Государственный регистрационный знак
        /// </summary>
        [Display(Name = "Рег. знак"), MaxLength(10)]
        public string RegNumber { get; set; }
        /// <summary>
        /// Масса без нагрузки (кг)
        /// </summary>
        [Required(ErrorMessage = "Required"), Display(Name = "Масса без нагрузки (кг)")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Категория ТС (СРТС или ПТС)")]
        public VehicleCategory Category { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Категория ТС (ОКП)")]
        public VehicleCategoryCommon CategoryCommon { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Марка шин"), MaxLength(30)]
        public string TyreManufacturer { get; set; }

        /// <summary>
        /// Разрешенная максимальная масса (кг)
        /// </summary>
        [Required(ErrorMessage = "Required"), Display(Name = "Разрешенная максимальная масса (кг)")]
        public int AllowedMaxWeight { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Тип топлива")]
        public FuelTypes FuelType { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Тип привода тормозной системы")]
        public BrakeTypes BrakeType { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Регистрационный документ")]
        public DocumentTypes DocumentType { get; set; }

        [Display(Name = "Собственник - иностранный гражданин")]
        public bool IsForeigner { get; set; }

        [Required(ErrorMessage = "Required"), Display(Name = "Серия"), StringLength(4)]
        public string DocumentSeries { get; set; }
        [Required(ErrorMessage = "Required"), Display(Name = "Номер"), StringLength(6)]
        public string DocumentNumber { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required"), Display(Name = "Дата выдачи")]
        public DateTime DocumentIssueDate { get; set; }
        [Required(ErrorMessage = "Required"), Display(Name = "Выдан кем"), MaxLength(50)]
        public string DocumentIssuer { get; set; }

        [Display(Name = "Примечание"), MaxLength(250)]
        public string Note { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Text)]
        [Display(Name = "Срок действия карты")]
        public DateTime ExpirationDate { get; set; }
    }
}