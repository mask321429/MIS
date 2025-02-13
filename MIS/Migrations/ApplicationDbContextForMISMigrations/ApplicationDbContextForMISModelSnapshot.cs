﻿// <auto-generated />
using System;
using MIS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MIS.Migrations.ApplicationDbContextForMISMigrations
{
    [DbContext(typeof(ApplicationDbContextForMIS))]
    partial class ApplicationDbContextForMISModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MIS.Data.Models.Record", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ACTUAL")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ID_PARENT")
                        .HasColumnType("uuid");

                    b.Property<string>("MKB_CODE")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MKB_NAME")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("REC_CODE")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ROOT")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.ToTable("Records");
                });
#pragma warning restore 612, 618
        }
    }
}
