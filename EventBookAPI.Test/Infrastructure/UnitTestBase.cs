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
        protected readonly ControllerContext _controllerContext;
        protected readonly DataContext _resultContext;
        protected readonly DataContext _seedContext;

        public UnitTestBase()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _seedContext = new(options);
            _context = new(options);
            _resultContext = new(options);

            _seedContext.Database.EnsureCreated();

            _controllerContext = new();
            _controllerContext.HttpContext = new DefaultHttpContext();
            _controllerContext.HttpContext.Request.Scheme = "https";
            _controllerContext.HttpContext.Request.Host = new("localhost", 5001);
            _controllerContext.HttpContext.User = new(
                new ClaimsIdentity(new[]
                {
                    new Claim("id", TestDbHelper.GuidIdString(1))
                }));
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