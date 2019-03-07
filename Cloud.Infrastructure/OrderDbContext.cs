﻿using Cloud.Core;
using Microsoft.EntityFrameworkCore;

namespace Cloud.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext() { }

        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>(order =>
            {
                order.HasKey(x => x.Id);
                order.Property(x => x.Description).IsRequired();
                order.Property(x => x.Amount).IsRequired();
            });
        }
    }
}
