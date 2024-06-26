﻿// <auto-generated />
using System;
using DiplomaApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240424053621_FirstMigration")]
    partial class FirstMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DiplomaApi.DataRepository.Models.IpAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Intent")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "intent");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "ip");

                    b.HasKey("Id");

                    b.ToTable("IpAddresses");
                });

            modelBuilder.Entity("DiplomaApi.DataRepository.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasAnnotation("Relational:JsonPropertyName", "created_at");

                    b.Property<double>("DdosProbability")
                        .HasColumnType("double precision")
                        .HasAnnotation("Relational:JsonPropertyName", "ddos_probability");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "ip");

                    b.Property<int>("RequestCount")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "request_count");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("DiplomaApi.DataRepository.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVerify")
                        .HasColumnType("boolean");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DiplomaApi.DataRepository.Models.Log", b =>
                {
                    b.HasOne("DiplomaApi.DataRepository.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
