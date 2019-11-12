using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibraryData
{
    public class LibraryContext :DbContext
    {
        public LibraryContext(DbContextOptions options) : base(options) { }


        /**
         * DbSet<TEnitity> represents an entity used for CRUD operations.
         * Intuitively, think of DbCOntext as representing your database,
         * and DbSet<SomeEntity> as representing a table in that db
         * */
        public DbSet<Patron> Patrons { get; set; }

    }
}
