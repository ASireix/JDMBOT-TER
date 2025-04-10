﻿// <auto-generated />
using System;
using BotJDM.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BotJDM.Migrations
{
    [DbContext(typeof(BotDBContext))]
    partial class BotDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BotJDM.Database.Entities.NodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("C")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("InfoId")
                        .HasColumnType("int");

                    b.Property<double>("Level")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TouchDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("W")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Nodes", (string)null);
                });

            modelBuilder.Entity("BotJDM.Database.Entities.RelationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("C")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("InfoId")
                        .HasColumnType("int");

                    b.Property<int>("Node1")
                        .HasColumnType("int");

                    b.Property<int>("Node2")
                        .HasColumnType("int");

                    b.Property<double>("Nw")
                        .HasColumnType("float");

                    b.Property<int>("Probability")
                        .HasColumnType("int");

                    b.Property<DateTime>("TouchDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<double>("W")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Relations", (string)null);
                });

            modelBuilder.Entity("BotJDM.Database.Entities.UserEntity", b =>
                {
                    b.Property<decimal>("DiscordUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("TrustFactor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DiscordUserId");

                    b.ToTable("Users", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
