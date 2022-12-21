using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;

namespace RefreshNorthcrestDataFromPlanningCenter.Data
{
    public class NorthcrestDbContext : DbContext
    {
        public DbSet<Sermon> Sermons { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<GeneralSong> GeneralSongs { get; set; }
        public DbSet<Plan_ForSongs> Plan_ForSongs { get; set; }
        public DbSet<PlanSong> PlanSongs { get; set; }
        public DbSet<SongNote> SongNotes { get; set; }
        public DbSet<SongAttachment> SongAttachments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder
                .UseSqlServer(GetConnectionStringForNorthcrestDbContext(),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: System.TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.Attachment", b =>
            {
                b.Property<int>("AttachmentId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttachmentId"), 1L, 1);

                b.Property<string>("ContentType")
                    .HasColumnType("varchar(100)");

                b.Property<bool>("Downloadable")
                    .HasColumnType("bit");

                b.Property<byte[]>("File")
                    .HasColumnType("varbinary(max)")
                    .IsRequired();

                b.Property<string>("FileName")
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                b.Property<int>("FileSize")
                    .HasColumnType("int");

                b.Property<string>("FileType")
                    .HasColumnType("varchar(25)")
                    .IsRequired();

                b.Property<bool>("HasPreview")
                    .HasColumnType("bit");

                b.Property<int>("SermonId")
                    .HasColumnType("int");

                b.Property<string>("Url")
                    .HasColumnType("varchar(5000)");

                b.HasKey("AttachmentId");

                b.HasIndex("SermonId");

                b.ToTable("Attachments");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.Sermon", b =>
            {
                b.Property<int>("SermonId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SermonId"), 1L, 1);

                b.Property<string>("Description")
                    .HasColumnType("varchar(5000)");

                b.Property<int>("PlanId")
                    .HasColumnType("int");

                b.Property<DateTime>("SermonDateTime")
                    .HasColumnType("datetime2(7)");

                b.Property<string>("Speaker")
                    .HasColumnType("varchar(1000)");

                b.Property<string>("Title")
                    .HasColumnType("varchar(200)");

                b.Property<string>("Type")
                    .HasColumnType("varchar(50)");

                b.HasKey("SermonId");

                b.ToTable("Sermons");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.GeneralSong", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                b.Property<int>("SongId")
                    .HasColumnType("int");

                b.Property<int>("ArrangementId")
                    .HasColumnType("int");

                b.Property<string>("SongName")
                    .HasColumnType("varchar(500)");

                b.Property<string>("ArrangementName")
                    .HasColumnType("varchar(1000)");

                b.Property<string>("Author")
                    .HasColumnType("varchar(2000)");

                b.Property<string>("Copyright")
                    .HasColumnType("varchar(2000)");

                b.Property<int>("Length")
                    .HasColumnType("int");

                b.Property<string>("Themes")
                    .HasColumnType("varchar(2000)");

                b.Property<DateTime>("LastScheduledDateTime")
                    .HasColumnType("datetime2(7)");

                b.HasKey("Id");

                b.ToTable("GeneralSongs");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.SongAttachment", b =>
            {
                b.Property<int>("SongAttachmentId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SongAttachmentId"), 1L, 1);

                b.Property<string>("ContentType")
                    .HasColumnType("varchar(100)");

                b.Property<bool>("Downloadable")
                    .HasColumnType("bit");

                b.Property<byte[]>("File")
                    .HasColumnType("varbinary(max)");

                b.Property<string>("FilePath")
                    .HasColumnType("varchar(500)");

                b.Property<string>("FileName")
                    .HasColumnType("varchar(100)")
                    .IsRequired();

                b.Property<int>("FileSize")
                    .HasColumnType("int");

                b.Property<string>("FileType")
                    .HasColumnType("varchar(25)")
                    .IsRequired();

                b.Property<bool>("HasPreview")
                    .HasColumnType("bit");

                b.Property<string>("Url")
                    .HasColumnType("varchar(5000)");

                b.HasKey("SongAttachmentId");

                b.HasIndex("PlanSongId");

                b.ToTable("SongAttachments");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.SongNote", b =>
            {
                b.Property<int>("PlanSongId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SongNoteId"), 1L, 1);


                b.Property<string>("CategoryName")
                    .HasColumnType("varchar(50)");

                b.Property<string>("Content")
                    .HasColumnType("varchar(1000)");

                b.HasKey("SongNoteId");

                b.HasIndex("PlanSongId");

                b.ToTable("SongNotes");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.PlanSong", b =>
            {
                b.Property<int>("PlanSongId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlanSongId"), 1L, 1);

                b.Property<int>("SongId")
                    .HasColumnType("int");

                b.Property<int>("ArrangementId")
                    .HasColumnType("int");

                b.Property<string>("SongName")
                    .HasColumnType("varchar(500)");

                b.Property<string>("ArrangementName")
                    .HasColumnType("varchar(1000)");

                b.Property<string>("Author")
                    .HasColumnType("varchar(2000)");

                b.Property<string>("Copyright")
                    .HasColumnType("varchar(2000)");

                b.Property<int>("Length")
                    .HasColumnType("int");

                b.Property<string>("KeyName")
                    .HasColumnType("varchar(25)");

                b.Property<string>("Description")
                    .HasColumnType("varchar(2000)");

                b.Property<int>("Sequence")
                    .HasColumnType("int");

                b.Property<string>("Notes")
                    .HasColumnType("varchar(5000)");

                b.HasKey("PlanSongId");

                b.HasIndex("Plan_ForSongsId");

                b.ToTable("PlanSongs");
            });

            modelBuilder.Entity("RefreshNorthcrestDataFromPlanningCenter.Domain.Plan_ForSongs", b =>
            {
                b.Property<int>("Plan_ForSongsId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Plan_ForSongsId"), 1L, 1);

                b.Property<int>("PlanId")
                    .HasColumnType("int");

                b.Property<string>("Type")
                    .HasColumnType("varchar(50)");

                b.Property<DateTime>("PlanDateTime")
                    .HasColumnType("datetime2(7)");

                b.HasKey("Plan_ForSongsId");

                b.ToTable("Plan_ForSongs");
            });
        }

        private string GetConnectionStringForNorthcrestDbContext()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            return configuration.GetConnectionString("Northcrest_DB_ConnectionString");
        }
    }
}
