using AutoMapper;
using System.Linq;
using System.Diagnostics;
using WebAdvert.Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Home;
using System.Collections.Generic;
using WebAdvert.Web.ServiceClients;
using Microsoft.AspNetCore.Authorization;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        public ISearchApiClient _searchApiClient { get; }
        public IMapper _mapper { get; }
        public IAdvertApiClient _apiClient { get; }

        public HomeController(ISearchApiClient searchApiClient, IMapper mapper, IAdvertApiClient apiClient)
        {
            _searchApiClient = searchApiClient;
            _mapper = mapper;
            _apiClient = apiClient;
        }

        [Authorize]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            var allAds = await _apiClient.GetAllAsync().ConfigureAwait(false);
            var allViewModels = allAds.Select(x => _mapper.Map<IndexViewModel>(x));

            return View(allViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            var viewModel = new List<SearchViewModel>();
            var searchResult = await _searchApiClient.Search(keyword).ConfigureAwait(false);

            searchResult.ForEach(advertDoc =>
            {
                var viewModelItem = _mapper.Map<SearchViewModel>(advertDoc);
                viewModel.Add(viewModelItem);
            });

            return View("Search", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}