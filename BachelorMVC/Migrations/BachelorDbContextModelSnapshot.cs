using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BachelorMVC.Persistence;

namespace BachelorMVC.Migrations
{
    [DbContext(typeof(BachelorDbContext))]
    partial class BachelorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BachelorMVC.Models.Bruker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Etternavn");

                    b.Property<string>("Fornavn");

                    b.Property<string>("unikID");

                    b.HasKey("Id");

                    b.ToTable("Bruker");
                });

            modelBuilder.Entity("BachelorMVC.Models.Dokumenter", b =>
                {
                    b.Property<int>("DokumentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("signatur");

                    b.HasKey("DokumentID");

                    b.ToTable("Dokumenter");
                });
        }
    }
}
