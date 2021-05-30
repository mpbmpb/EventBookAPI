using System;
using EventBookAPI.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace EventBookAPI.Test.Infrastructure
{
    public class UnitTestBase : IDisposable
    {
        protected readonly DataContext _seedContext;
        protected readonly DataContext _context;
        protected readonly DataContext _resultContext;

        public UnitTestBase()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _seedContext = new DataContext(options);
            _context = new DataContext(options);
            _resultContext = new DataContext(options);

            _seedContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _seedContext.Database.EnsureDeleted();
            _context.Database.EnsureDeleted();
            _resultContext.Database.EnsureDeleted();
            
            _seedContext?.Dispose();
            _context?.Dispose();
            _resultContext?.Dispose();
        }
    }
}