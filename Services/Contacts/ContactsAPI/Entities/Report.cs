﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLibrary.Domains;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContactsAPI.Entities
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }
        public DateTime RaporTalepTarihi { get; set; }
        public ReportStatus RaporDurumu { get; set; }
    }

    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(x => x.ReportId);
            builder.Property(x => x.RaporDurumu).IsRequired();
            builder.Property(x=>x.RaporTalepTarihi).IsRequired();
        }
    }
}
