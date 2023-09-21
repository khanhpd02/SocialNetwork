using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Entity;


public class DataContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<PinCode> PinCode { get; set; }



    private readonly IConfiguration Configuration;

    public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // in memory database used for simplicity, change to a real db for production applications
        //optionsBuilder.UseInMemoryDatabase("TestDb");
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
