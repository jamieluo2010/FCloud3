@@
     public class FCloudContext : DbContext
     {
@@
         public DbSet<User> Users { get; set; }
@@
         public DbSet<UserConfig> UserConfigs { get; set; }
+        public DbSet<FCloud3.Entities.Transport.TransportItem> TransportItems { get; set; }
+        // Transport items: stores per-user uploaded HTML snippets (FileUrl points to static file under wwwroot/transports/{ownerId}/)
     }
