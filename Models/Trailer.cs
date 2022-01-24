﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Models
{
    public class Trailer
    {
        public Trailer()
        {
            Orders = new HashSet<Order>();
            Colors = new HashSet<Color>();
        }

        public int Id { get; set; }

        [Display(Name = "Гос. номер")]
        public string LicensePlate { get; set; }

        [Display(Name = "Грузоподъемность")]
        public int Carrying { get; set; }

        [Display(Name = "Цена")]
        public int Price { get; set; }

        //[Display(Name = "Цвет")]
        //public int ColorId { get; set; }

        public virtual ICollection<Color> Colors { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
