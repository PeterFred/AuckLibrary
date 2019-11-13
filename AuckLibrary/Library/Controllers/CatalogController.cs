using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Library.Models.Catalog;
using LibraryData.Models;
using System.Collections.Generic;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        private ICheckout _checkouts;

        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _assets = assets;
            _checkouts = checkouts;
        }

        //Controller uses the service set-up (LibraryAssetService) to select into the particular
        //viewModel (AssetIndexListingModel -> AssetIndexModel) to be returned to be rendered
        public IActionResult Index()
        {
            
            IEnumerable<LibraryAsset> assetModels = _assets.GetAll();
            IEnumerable<AssetIndexListingModel> listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id= result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    Dewey =  _assets.GetDeweyIndex(result.Id),
                    Type = _assets.GetType(result.Id),
                    Title = result.Title,
                });

            AssetIndexModel model = new AssetIndexModel()
            {
                Assets = listingResult
            };
            return View(model);
        }

        //Returns the details for a selected asset
       public IActionResult Detail(int id)
        {
            //Selects the required asset
            LibraryAsset asset = _assets.GetById(id);

            var currentHolds = _checkouts.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id).ToString("d"),
                    PatronName = _checkouts.GetCurrentHoldPatronName(a.Id)
                });

            //Creates the ViewModel
            //Populate the AssetDetail model with:
            // LibraryAsset properties OR
            // services from the _assets ILibrary service
            AssetDetailModel model = new AssetDetailModel()
            {
                AssetId = id,
                Title = asset.Title,
                Year = asset.Year,
                Status = asset.Status.Name,
                Cost = asset.Cost,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                Type = _assets.GetType(id),
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id),
                CheckoutHistory = _checkouts.GetCheckoutHistory(id),
                LatestCheckOut = _checkouts.GetLatestCheckout(id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds

            };

            return View(model);

        }
    }
}
