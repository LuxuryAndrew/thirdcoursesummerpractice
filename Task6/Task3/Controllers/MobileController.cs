using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Task3.Models;

namespace Task3.Controllers
{
    public class MobileController : ApiController
    {
        private FabricContext db = new FabricContext();

        // GET: api/Mobile
        [Authorize]
        public IEnumerable<string> Get()
        {
            string userId = User.Identity.GetUserId();
            IEnumerable<string> result = db.Fabrics.Where(c => c.UserId == userId).ToList().Select(fabric => Url.Content($"~/Content/fabrics/{fabric.Id}/thumbnail.png"));
            return result;
        }
    }
}
