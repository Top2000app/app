using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Top2000.Data.ClientDatabase;

namespace Top2000.Apps.CLI.Database;

public class Edition
{
    [Key]
    public required int Year { get; init; }
    public required DateTime StartUtcDateAndTime { get; init; }
    public required DateTime EndUtcDateAndTime { get; init; }
    public required bool HasPlayDateAndTime { get; init; }
    
    public ICollection<Listing> Listings { get; init; } = [];
}

public class Track
{
    [Key]
    public required int Id { get; init; }
    [MaxLength(100)]
    public required string Title { get; init; }
    [MaxLength(100)]
    public required string Artist { get; init; }
    public required int RecordedYear { get; init; }

    [MaxLength(100)]
    public string? SearchTitle { get; init; }
    
    [MaxLength(100)]
    public string? SearchArtist { get; init; }
    
    public ICollection<Listing> Listings { get; init; } = [];
}

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
    
    public required Track Track { get; init; }
    public required Edition Edition { get; init; }
}


public class Top2000DbContext : DbContext
{
    private readonly Top2000ServiceBuilder _top2000ServiceBuilder;

    public Top2000DbContext(Top2000ServiceBuilder top2000ServiceBuilder)
    {
        _top2000ServiceBuilder = top2000ServiceBuilder;
    }
    
    public DbSet<Edition> Editions { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Listing> Listings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var databasePath = Path.Combine(_top2000ServiceBuilder.Directory, _top2000ServiceBuilder.Name);
        var connectionString = $"Data Source={databasePath}";
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure singular table names
        modelBuilder.Entity<Edition>().ToTable("Edition");
        modelBuilder.Entity<Track>().ToTable("Track");
        modelBuilder.Entity<Listing>().ToTable("Listing");

        // Configure composite key for Listing
        modelBuilder.Entity<Listing>()
            .HasKey(l => new { l.TrackId, l.EditionId });

        // Map EditionId property to Edition column in database
        modelBuilder.Entity<Listing>()
            .Property(l => l.EditionId)
            .HasColumnName("Edition");

        // Configure relationships
        modelBuilder.Entity<Listing>()
            .HasOne(l => l.Track)
            .WithMany(t => t.Listings)
            .HasForeignKey(l => l.TrackId);

        modelBuilder.Entity<Listing>()
            .HasOne(l => l.Edition)
            .WithMany(e => e.Listings)
            .HasForeignKey(l => l.EditionId)
            .HasPrincipalKey(e => e.Year);

        // Add missing foreign key constraint for Track
        modelBuilder.Entity<Listing>()
            .HasIndex(l => l.TrackId);
    }
}