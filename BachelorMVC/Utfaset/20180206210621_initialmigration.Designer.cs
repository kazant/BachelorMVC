using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BachelorMVC.Persistence;

namespace BachelorMVC.Migrations
{
    [DbContext(typeof(BachelorDbContext))]
    [Migration("20180206210621_initialmigration")]
    partial class initialmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
