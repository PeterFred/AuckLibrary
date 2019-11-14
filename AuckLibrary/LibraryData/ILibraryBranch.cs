using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<Patron> GetPatrons(int branchId);
        IEnumerable<LibraryAsset> GetAssets(int branchId);
        LibraryBranch Get(int branchId);
        IEnumerable<string> GetBranchHours(int branchId);
        void Add(LibraryBranch newBranch);
        bool isBranchOpen(int branchId);
        int GetAssetCount(int branchId);
        int GetPatronCount(int branchId);
        decimal GetAssetsValue(int id);
    }
}
