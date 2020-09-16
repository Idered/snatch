using System;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Snatch
{
  public class DbClient : DbContext
  {
    public DbSet<Entry> Entries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string fullPath = $"{localPath}\\Snatch";
      string destination = $"{fullPath}\\Snatch.db";

      Debug.WriteLine($"destination: {destination}");

      if (!Directory.Exists(fullPath))
      {
        Directory.CreateDirectory(fullPath);
      }

      if (!File.Exists(destination))
      {
        string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
        int index = location.LastIndexOf("\\");
        string source = $"{location.Substring(0, index)}\\Snatch.db";
        Debug.WriteLine($"source: {source}");
        File.Copy(source, destination);
      }

      optionsBuilder.UseSqlite($"Data Source={destination}");
      base.OnConfiguring(optionsBuilder);
    }
  }
}
