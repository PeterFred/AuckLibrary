using System.Collections.Generic;
using System.Linq;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            IEnumerable<BranchDetailModel> branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                Address = b.Address,
                Name = b.Name,
                OpenDate = b.OpenDate.ToString(),
                Telephone = b.Telephone,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                IsOpen = _branch.isBranchOpen(b.Id),
                NumberOfPatrons = _branch.GetPatrons(b.Id).Count(), //Consider refactoring GetPatrons to just a count method
                NumberOfAssets = _branch.GetAssets(b.Id).Count()
            });

            BranchIndexModel model = new BranchIndexModel()
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = _branch.Get(id);
            var model = new BranchDetailModel
            {
                Name = branch.Name,
                Description = branch.Description,
                Address = branch.Address,
                Telephone = branch.Telephone,
                OpenDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                NumberOfPatrons = _branch.GetPatronCount(id),
                NumberOfAssets = _branch.GetAssetCount(id),
                TotalAssetValue = _branch.GetAssetsValue(id),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id)
            };

            return View(model);
        }


    }
}