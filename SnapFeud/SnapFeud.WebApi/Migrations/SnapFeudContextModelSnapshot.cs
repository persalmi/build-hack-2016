using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SnapFeud.WebApi.Models;

namespace SnapFeud.WebApi.Migrations
{
    [DbContext(typeof(SnapFeudContext))]
    partial class SnapFeudContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SnapFeud.WebApi.Models.Challenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Tag");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SnapFeud.WebApi.Models.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChallengeExpireTime");

                    b.Property<int?>("CurrentChallengeId");

                    b.Property<int?>("PlayerId");

                    b.Property<int>("Score");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SnapFeud.WebApi.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HighScore");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("SnapFeud.WebApi.Models.Game", b =>
                {
                    b.HasOne("SnapFeud.WebApi.Models.Challenge")
                        .WithMany()
                        .HasForeignKey("CurrentChallengeId");

                    b.HasOne("SnapFeud.WebApi.Models.Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");
                });
        }
    }
}
