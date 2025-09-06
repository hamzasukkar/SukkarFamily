using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SukkarFamily.Models;
using SukkarFamily.ViewModel;

namespace SukkarFamily.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DB db;

        public HomeController(ILogger<HomeController> logger, DB db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult IndexOld()
        {
            return View(db.News.ToList());
        }

        public IActionResult Index()
        {
            var viewModel = new HomeIndexViewModel
            {
                News = db.News.ToList(),
                FamilyStatistics = GetFamilyStatistics()
            };
            return View(viewModel);
        }

        public IActionResult IndexArabic()
        {
            var viewModel = new HomeIndexViewModel
            {
                News = db.News.ToList(),
                FamilyStatistics = GetFamilyStatistics()
            };
            return View("IndexArabic", viewModel);
        }

        private FamilyStatisticsViewModel GetFamilyStatistics()
        {
            var allPersones = db.persones.Include(p => p.children).ToList();
            
            // Calculate basic statistics
            var totalMembers = allPersones.Count;
            var generations = allPersones.Select(p => p.Generation).Distinct().Count();
            
            // Find root member (typically generation 1 or lowest generation)
            var rootMember = allPersones.OrderBy(p => p.Generation).FirstOrDefault();
            var lastGenMember = allPersones.OrderByDescending(p => p.Generation).FirstOrDefault();
            
            // Calculate members by generation for analysis
            var generationCounts = allPersones
                .GroupBy(p => p.Generation)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Find members with most children (family leaders)
            var membersWithChildren = allPersones
                .Where(p => p.children != null && p.children.Any())
                .OrderByDescending(p => p.children.Count)
                .Take(3)
                .ToList();

            // Extract unique locations from titles (if any location info is in titles)
            var uniqueLocations = allPersones
                .Where(p => !string.IsNullOrEmpty(p.title))
                .Select(p => ExtractLocationFromTitle(p.title))
                .Where(loc => !string.IsNullOrEmpty(loc))
                .Distinct()
                .Count();

            return new FamilyStatisticsViewModel
            {
                TotalMembers = totalMembers,
                TotalGenerations = generations > 0 ? generations : 1,
                // Estimate gender split (can be updated when gender field is added)
                MaleMembers = (int)(totalMembers * 0.52), // Rough estimate
                FemaleMembers = (int)(totalMembers * 0.48),
                OldestMemberName = rootMember?.name ?? "غير محدد",
                YoungestMemberName = lastGenMember?.name ?? "غير محدد",
                LivingMembers = totalMembers, // Assume all are living unless death info is added
                UniqueLocations = Math.Max(uniqueLocations, 4), // Default to known locations if no data
                EarliestBirthYear = DateTime.Now.AddYears(-80), // Estimated for oldest generation
                LatestBirthYear = DateTime.Now.AddYears(-20) // Estimated for youngest generation
            };
        }

        private string ExtractLocationFromTitle(string title)
        {
            if (string.IsNullOrEmpty(title)) return null;
            
            // Common Syrian/Lebanese city names that might appear in titles
            var knownCities = new[] { "دمشق", "بيروت", "حلب", "الصالحية", "الميدان", "بعلبك", "حمص", "اللاذقية" };
            
            foreach (var city in knownCities)
            {
                if (title.Contains(city))
                    return city;
            }
            
            return null;
        }

        public IActionResult News()
        {
            return View(db.News.ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
