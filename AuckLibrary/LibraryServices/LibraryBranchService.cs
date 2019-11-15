using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;

        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return GetAll().FirstOrDefault(l => l.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(l => l.LibraryAssets)
                .Include(l => l.Patrons);
        }

        public int GetAssetCount(int branchId)
        {
            return Get(branchId).LibraryAssets.Count();
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.LibraryAssets)
                .FirstOrDefault(b => b.Id == branchId).LibraryAssets;
        }

        public decimal GetAssetsValue(int id)
        {
            var assetsValue = GetAssets(id).Select(a => a.Cost);
            return assetsValue.Sum();
        }

        //Use dataHelper service to return a string of branch hours
        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(h => h.Branch.Id == branchId);
            return DataHelpers.HumanizeBusHours(hours);
        }

        public int GetPatronCount(int branchId)
        {
            return Get(branchId).Patrons.Count();
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .FirstOrDefault(b => b.Id == branchId)
                .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            return true;
            //int currentTimeHour = DateTime.Now.Hour;
            //int currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1; //DB is indexed 1-7, not 0-6, hence +1
            //var hours = _context.BranchHours.Where(h => h.Branch.Id == branchId);

            //BranchHours daysHours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayOfWeek);

            //return  currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
        }
    }
}
