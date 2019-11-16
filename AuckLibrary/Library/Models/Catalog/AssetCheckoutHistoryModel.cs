using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Catalog
{
    public class AssetCheckoutHistoryModel
    {
        public int Id { get; set; }
        [Required]
        public LibraryAsset LibraryAsset { get; set; }

        [Required]
        public LibraryCard LibraryCard { get; set; }

        [Required]
        public DateTime CheckedOut { get; set; }

        //'?' means explicitly nullable
        public DateTime? CheckedIn { get; set; }

        public string PatronName { get; set; }

        public int LibraryCardId { get; set; }
    }
}
