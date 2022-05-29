using WebAdvert.Web.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebAdvert.Web.ServiceClients
{
    public interface ISearchApiClient
    {
        Task<List<AdvertType>> Search(string keyword);
    }
}