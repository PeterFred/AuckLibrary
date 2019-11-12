using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    /**
     * LibraryAssestService links the db objects (LibraryContext)
     * with the front end (return calls)
     */

    public class LibraryAssetService : ILibraryAsset
    {
        //db context object
        private LibraryContext _context;

        //db context Injected into constructor, allowing access to methods in dbContext
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public LibraryAsset GetById(int id)
        {
            //return _context.LibraryAssets
            //    .Include(asset => asset.Status)
            //    .Include(asset => asset.Location)
            //    .FirstOrDefault(e => e.Id == id);

            //REFACTOR ABOVE to below
            return GetAll()
                .FirstOrDefault(e => e.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            //return _context.LibraryAssets.FirstOrDefault(e => e.Id == id).Location;

            //REFACTOR ABOVE to below
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            //Discriminator for either books or videos, but only books have dewey
            if (_context.Books.Any(book => book.Id ==id))
            {
                return _context.Books
                    .FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else
            {
                return "";
            }
        }

        public string GetIsbn(int id)
        {
            //Discriminator for either books or videos, but only books have ISBN
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books
                    .FirstOrDefault(book => book.Id == id).ISBN;
            }
            else
            {
                return "";
            }
        }

        public LibraryCard GetLibraryCardByAssetId(int id)
        {
            return _context.LibraryCards
                .FirstOrDefault(c => c.Checkouts
                    .Select(a => a.LibraryAsset)
                    .Select(v => v.Id).Contains(id));
        }

        public string GetTitle(int id)
        {
            return GetAll()
                 .FirstOrDefault(e => e.Id == id).Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>()
                .Where(b => b.Id == id);

            return book.Any() ? "Book" : "Video";


        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(asset => asset.Id == id).Any();
            
            return isBook ? _context.Books.FirstOrDefault(e => e.Id == id).Author :
                _context.Videos.FirstOrDefault(e => e.Id == id).Director
                ?? "Unkown";
        }
    }
}
