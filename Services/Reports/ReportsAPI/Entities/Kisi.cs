﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReportsAPI.Entities
{
    public class Kisi : BaseEntity
    {
        public Guid Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Firma { get; set; }

        public List<Iletisim> IletisimBilgileri = new List<Iletisim>();
    }
}