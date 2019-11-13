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
            DateTime now = DateTime.Now;
            LibraryAsset item = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            //remove any existing checkouts on the item
            RemoveExistingCheckouts(assetId);
            
            // close any exisiting checkout history
            CloseExistingCheckoutHistory(assetId, now);
            
            // look for existing holds
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h=>h.LibraryAsset.Id == assetId);

            // if there are holds, checkout the item to the librarycard 
            //with the earliest hold, otherwise update the item status to avaiable

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
            }

            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();

        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckoutItem(assetId, card.Id);
        }

        public void CheckoutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                // Add logic here to handle feedback to the user
            }

            //Get the library item 
            LibraryAsset item = _context.LibraryAssets
               .FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            //Get the library Card
            LibraryCard libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);

            //Create a new checkout
            DateTime now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);
            //Update checkout history - this could be done using an sql trigger
            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        //Change how long an item can be checked out
        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        private bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(co =>co.LibraryAsset.Id == assetId).Any();
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

        //If no Hold is found, it will return null. ? is used to avoid nullException error
        public string GetCurrentHoldPatronName(int holdId)
        {
            Hold hold = _context.Holds
                .Include(h=>h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h=>h.Id==holdId);
            var cardId = hold?.LibraryCard.Id;

            Patron patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                 .Include(h => h.LibraryAsset)
                 .Include(h => h.LibraryCard)
                 .FirstOrDefault(h => h.Id == holdId)
                 .HoldPlaced;
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

            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId, now);  
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string statusType)
        {
            LibraryAsset item = _context.LibraryAssets
               .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == statusType);
        }

        //If found, remove any existing checkouts on that item
        //Grab the current checkout, and if !null, remove it
        private void RemoveExistingCheckouts(int assetId)
        {
            Checkout checkout = _context.Checkouts
                 .FirstOrDefault(co => co.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        private void CloseExistingCheckoutHistory(int assetId, DateTime now)
        {
            //close any existing checkout history
            CheckoutHistory history = _context.CheckoutHistories
                .FirstOrDefault(h => h.LibraryAsset.Id == assetId
                && h.CheckedOut == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }
        


        /**
         * Find the correct item from the db using _context
         * Update uses EntityFramework to start tracking the entity for update
         * Then update the item's status
         * Save changes
         */
        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");

            _context.SaveChanges();

        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            DateTime now = DateTime.Now;

            var asset = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(card => card.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card,
            };

            _context.Add(hold);
            _context.SaveChanges();
        }
    }
}
