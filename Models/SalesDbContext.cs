using System.Data.Entity;
using System.IO.Ports;

namespace Sales.Models
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext() : base("SalesDbContext") { }

        // DbSet for Users table
        public DbSet<User> Users { get; set; }

        // DbSet for City table (registering cities)
        public DbSet<City> Cities { get; set; }

        // DbSet for Country table
        public DbSet<Country> Countries { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<HSN> HSNs { get; set; }

        public DbSet<Count> Counts { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<Variety> Varieties { get; set; }

        public DbSet<KCS> KCSs { get; set; }

        public DbSet<WH> WHs { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Orderss> Orderss { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<ProductDetails> ProductDetails { get; set; }

        public DbSet<Despatch> Despatch { get; set; }

        public DbSet<DespatchDetails> DespatchDetails { get; set; }

        public DbSet<Invoice> Invoice { get; set; }

        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }

    }
}
