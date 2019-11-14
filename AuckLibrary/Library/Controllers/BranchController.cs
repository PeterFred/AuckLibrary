using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            branch = _branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                Address = b.Address,
                Name = b.Name,
                OpenDate = b.OpenDate.ToString(),
                Telephone = b.Telephone,
                Desciption = b.Description,
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

       
    }
}