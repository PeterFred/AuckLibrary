using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Library.Models.Catalog;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        public CatalogController(ILibraryAsset assets)
        {
            _assets = assets;
        }

        //Controller uses the service set-up (LibraryAssetService) to select into the particular
        //viewModel (AssetIndexListingModel -> AssetIndexModel) to be returned to be rendered
        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();
            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id= result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    Dewey =  _assets.GetDeweyIndex(result.Id),
                    Type = _assets.GetType(result.Id),
                    Title = result.Title,
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };
            return View(model);
        }
    }
}
