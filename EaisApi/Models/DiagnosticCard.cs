using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EaisApi.Models
{
    public class DiagnosticCard
    {
        [MaxLength(30)]
        public string CardId { get; set; }
        [Required, Display(Name = "Фамилия"), MaxLength(30)]
        public string Lastname { get; set; }
        [Required, Display(Name = "Имя"), MaxLength(30)]
        public string Firstname { get; set; }
        [Display(Name = "Отчество"), MaxLength(30)]
        public string Patronymic { get; set; }
        [StringLength(17), Display(Name = "ВИН")]
        public string VIN { get; set; }
        [Required, Display(Name = "Год выпуска ТС")]
        public int IssueYear { get; set; }

        /// <summary>
        /// Марка ТС
        /// </summary>
        [Required, Display(Name = "Марка ТС"), MaxLength(30)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Модель ТС
        /// </summary>
        [Required, Display(Name = "Модель ТС"), MaxLength(30)]
        public string Model { get; set; }

        [Display(Name = "Кузов №"), MaxLength(50)]
        public string BodyNumber { get; set; }
        [Display(Name = "Рама (Шасси) №"), MaxLength(50)]
        public string FrameNumber { get; set; }
        [Display(Name = "Пробег ТС (км)"), Range(100, int.MaxValue)]
        public int Running { get; set; }
        /// <summary>
        /// Государственный регистрационный знак
        /// </summary>
        [Display(Name = "Рег. знак"), MaxLength(10)]
        public string RegNumber { get; set; }
        /// <summary>
        /// Масса без нагрузки (кг)
        /// </summary>
        [Required, Display(Name = "Масса без нагрузки (кг)")]
        public int Weight { get; set; }

        [Required, Display(Name = "Категория ТС (СРТС или ПТС)")]
        public VehicleCategory Category { get; set; }

        [Required, Display(Name = "Категория ТС (ОКП)")]
        public VehicleCategoryCommon CategoryCommon { get; set; }

        [Required, Display(Name = "Марка шин"), MaxLength(30)]
        public string TyreManufacturer { get; set; }

        /// <summary>
        /// Разрешенная максимальная масса (кг)
        /// </summary>
        [Required, Display(Name = "Разрешенная максимальная масса (кг)")]
        public int AllowedMaxWeight { get; set; }

        [Required, Display(Name = "Тип топлива")]
        public FuelTypes FuelType { get; set; }

        [Required, Display(Name = "Тип привода тормозной системы")]
        public BrakeTypes BrakeType { get; set; }

        [Required, Display(Name = "Регистрационный документ")]
        public DocumentTypes DocumentType { get; set; }

        [Display(Name = "Собственник - иностранный гражданин")]
        public bool IsForeigner { get; set; }

        [Required, Display(Name = "Серия"), StringLength(4)]
        public string DocumentSeries { get; set; }
        [Required, Display(Name = "Номер"), StringLength(6)]
        public string DocumentNumber { get; set; }
        [DataType(DataType.Date)]
        [Required, Display(Name = "Дата выдачи")]
        public DateTime DocumentIssueDate { get; set; }
        [Required, Display(Name = "Выдан кем"), MaxLength(50)]
        public string DocumentIssuer { get; set; }

        [Display(Name = "Примечание"), MaxLength(250)]
        public string Note { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Срок действия")]
        public DateTime ExpirationDate { get; set; }
    }
}