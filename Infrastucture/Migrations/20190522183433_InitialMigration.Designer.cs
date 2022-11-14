﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Infrastucture.Migrations
{
    [DbContext(typeof(SzpekContext))]
    [Migration("20190522183433_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SzpekCore.Models.Sensor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Sensor");
                });
#pragma warning restore 612, 618
        }
    }
}