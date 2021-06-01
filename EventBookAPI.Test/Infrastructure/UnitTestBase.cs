using System;
using System.Security.Claims;
using EventBookAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBookAPI.Test.Infrastructure
{
    public class UnitTestBase : IDisposable
    {
        protected readonly DataContext _context;
        protected readonly DataContext _resultContext;
        protected readonly DataContext _seedContext;
        protected readonly ControllerContext _mockControllerContext;

        public UnitTestBase()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _seedContext = new(options);
            _context = new(options);
            _resultContext = new(options);

            _seedContext.Database.EnsureCreated();

            _mockControllerContext = TestHelper.GetMockControllerContext();
        }

        public void Dispose()
        {
            _seedContext.Database.EnsureDeleted();
            _context.Database.EnsureDeleted();
            _resultContext.Database.EnsureDeleted();

            _seedContext?.Dispose();
            _context?.Dispose();
            _resultContext?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}