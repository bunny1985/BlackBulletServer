using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotificationBackend.Models;

namespace NotificationBackend.Infrastrucuture.Database
{
    public class MyDbContext:  IdentityDbContext<User>
    {

        public DbSet<NotificationDbModel> Notifications { get; set; }
        public DbSet<TokenDbModel> FireBaseTokens { get; set; }
        public DbSet<SettingsDbModel> Settings { get; set; }
        
        public MyDbContext(DbContextOptions<MyDbContext> options ) : base(options)
        {
            
        }


    }
}