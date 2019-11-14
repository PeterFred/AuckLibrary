using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class PatronController: Controller
    {
        private IPatron _patron;

        public PatronController(IPatron patron)
        {
            _patron = patron;
        }
    }
}
