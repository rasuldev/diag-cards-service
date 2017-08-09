using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EaisApi.Models
{
    public class VehicleInfo
    {
        [Display(Name="Фамилия")]
        [Required]
        public string Lastname { get; set; }
        [Display(Name = "Имя")]
        [Required]
        public string Firstname { get; set; }
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }
        [StringLength(17)]
        public string VIN { get; set; }
        [Display(Name="Год выпуска ТС")]
        [Required]
        public int IssueYear { get; set; }

        /// <summary>
        /// Марка ТС
        /// </summary>
        [Display(Name = "Марка ТС")]
        [Required]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Модель ТС
        /// </summary>
        [Display(Name = "Модель ТС")]
        [Required]
        public string Model { get; set; }

        [Display(Name="Кузов №")]
        public string BodyNumber { get; set; }
        [Display(Name="Рама (Шасси) №")]
        public string FrameNumber { get; set; }
        [Display(Name="Пробег ТС (км)"), Range(100, int.MaxValue)]
        public int Running { get; set; }
        [Display(Name = "Государственный регистрационный знак")]
        public string RegNumber { get; set; }
        
        /// <summary>
        /// Масса без нагрузки (кг)
        /// </summary>
        [Display(Name = "Масса без нагрузки (кг)")]
        [Required]
        public int Weight { get; set; }

        [Required, Display(Name = "Категория ТС (СРТС или ПТС)")]
        public VehicleCategory Category { get; set; }

        [Required, Display(Name = "Категория ТС (ОКП)")]
        public VehicleCategoryCommon CategoryCommon { get; set; }

        [Required, Display(Name = "Марка шин")]
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
        [Required, Display(Name = "Дата выдачи")]
        public DateTime DocumentIssueDate { get; set; }
        [Required, Display(Name = "Выдан кем")]
        public string DocumentIssuer { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [Display(Name = "Срок действия")]
        public DateTime ExpirationDate { get; set; }
    }
}