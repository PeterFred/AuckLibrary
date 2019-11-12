using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int checkoutId);
        void AddChecout(Checkout checkout);
        void CheckoutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);
        IEnumerable<CheckoutHistory> GetCheckoutHistoriy(int id);

        void PlaceHold(int assetId, int libraryCardId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        void MarkLost(int assetId);
        void MarkFound(int assetId);
    }
}
