using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using LibraryData;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void AddChecout(Checkout checkout)
        {
            _context.Add(checkout);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckoutItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(checkout => checkout.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistoriy(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentHoldPatronName(int id)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.Id == id);
        }

        //Get all the checkout items, order into date order, 
        //then take the first one from the list
        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public void MarkFound(int assetId)
        {
            DateTime now = DateTime.Now;

            LibraryAsset item = _context.LibraryAssets
               .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == "Avilable");

            //If found, remove any existing checkouts on that item
            //Grab the current checkout, and if !null, remove it

            Checkout checkout = _context.Checkouts
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }

            //close any existing checkout history
            CheckoutHistory history = _context.CheckoutHistories
                .FirstOrDefault(h => h.LibraryAsset.Id == assetId 
                && h.CheckedOut == null);

            if(history != null)
            {
                _context.Remove(checkout);
           }
            _context.SaveChanges();
        }


        /**
         * Find the correct item from the db using _context
         * Update uses EntityFramework to start tracking the entity for update
         * Then update the item's status
         * Save changes
         */
        public void MarkLost(int assetId)
        {
            LibraryAsset item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == "Lost");

            _context.SaveChanges();

        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }
    }
}
