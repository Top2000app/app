# Top 2000 CLI

A command-line interface application for exploring and analyzing the NPO Radio 2 Top 2000 music list data.

## About

The Top 2000 is an annual music event on NPO Radio 2 (Netherlands) where listeners vote for their favorite songs of all time. This CLI tool provides an easy way to query, analyze, and export Top 2000 data directly from your terminal.

## Installation

Install as a global .NET tool:

```bash
dotnet tool install -g top2000.cli
```

Or run directly from source:

```bash
dotnet run --project Top2000.Apps.CLI.csproj
```

## Usage

```bash
top2000 [command] [options]
```

## Commands

### Show Commands

Display Top 2000 data in your terminal with beautiful formatted tables.

#### `show now`

Show the currently playing Top 2000 song (when the broadcast is live).

```bash
top2000 show now
```

#### `show editions`

List all available Top 2000 editions with their broadcast dates and duration.

```bash
top2000 show editions
```

#### `show edition <year>`

Display the Top 2000 listing for a specific year.

```bash
top2000 show edition 2025
```

**Options:**

- `--top <number>` - Show only the top N tracks
- `--skip <number>` - Skip N tracks from the start
- `--take <number>` - Take N tracks from the listing
- `--new` - Show only tracks that are new to the Top 2000 this edition
- `--recurring` - Show only tracks that are back in the Top 2000 after being absent
- `--risers` - Show only tracks that have increased in position from the previous edition
- `--fallers` - Show only tracks that have decreased in position from the previous edition
- `--held` - Show only tracks that have maintained the same position from the previous edition
- `--order <ordering>` - Order by: Rank, Title, Artist, Delta, RankDescending, TitleDescending, ArtistDescending, DeltaDescending (default: Rank)

**Examples:**

```bash
# Show top 10 tracks of 2025
top2000 show edition 2025 --top 10

# Show new entries in 2025
top2000 show edition 2025 --new

# Show biggest risers, ordered by delta
top2000 show edition 2025 --risers --order DeltaDescending

# Show positions 100-200
top2000 show edition 2025 --skip 99 --take 100
```

### Search Commands

Search for tracks and artists in the Top 2000 database.

#### `search`

Search for tracks or artists by query string.

```bash
top2000 search "Bohemian Rhapsody"
top2000 search "Queen"
```

**Options:**

- `--showIds, -i` - Show track IDs in the results

### Stats Commands

View statistics and insights about Top 2000 editions.

#### `stats edition`

Show comprehensive statistics for a specific edition, including:
- Number of tracks that increased/decreased in position
- Highest climbing and falling tracks
- New entries and returning tracks
- Unchanged tracks

```bash
top2000 stats edition 2025
```

### Export Commands

Export Top 2000 data to various formats for further analysis or integration.

#### `export json`

Export all Top 2000 data to JSON format.

```bash
top2000 export json --output data.json
```

#### `export csv`

Export all Top 2000 data to CSV format.

```bash
top2000 export csv --output data.csv
```

#### `export api`

Export data to a static API structure (multiple JSON files organized by endpoint).

```bash
top2000 export api --output ./api
```

## Features

- 🎵 **Real-time tracking** - See what's playing now during the live broadcast
- 📊 **Rich statistics** - Analyze trends, movers, and new entries
- 🔍 **Fast search** - Quickly find tracks and artists
- 📁 **Multiple export formats** - JSON, CSV, and static API
- 🎨 **Beautiful terminal UI** - Powered by Spectre.Console
- 💾 **Local SQLite database** - Fast queries with automatic updates

## Data Source

The CLI automatically initializes and updates its local database on startup. Data is sourced from the official Top 2000 sources.

## Technical Details

- **Framework:** .NET 10.0
- **Command-line parsing:** System.CommandLine
- **Terminal UI:** Spectre.Console
- **Database:** SQLite with Entity Framework Core

## License

Copyright 2025 (c) Rick Neeft Development.

## Author

Rick Neeft

---

*Made with ❤️ for Top 2000 fans*

