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
            var branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                Name = b.Name,
                NumberOfAssets = _branch.GetAssetCount(b.Id),
                NumberOfPatrons = _branch.GetPatronCount(b.Id),
                IsOpen = _branch.IsBranchOpen(b.Id),              
                Address = b.Address,
                OpenDate = b.OpenDate.ToString(),
                Telephone = b.Telephone,
                Description = b.Description,
                ImageUrl = b.ImageUrl
            }).ToList();

            var model = new BranchIndexModel()
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