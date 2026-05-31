using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SukkarFamily.Models;

namespace SukkarFamily.Controllers
{
    public class PersoneController : Controller
    {
        private DB db;
        public PersoneController(DB db)
        {
            this.db = db;
        }
        // GET: Persone
        public ActionResult Index(int page = 1, int pageSize = 20, string searchTerm = "", int? searchId = null, int? generation = null)
        {
            // Get all persons for statistics (we still need full count)
            var allPersones = db.persones.Include("Parent").ToList();

            // Apply filters
            var filteredPersones = allPersones.AsEnumerable();

            // Search by name, title, or national number
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                filteredPersones = filteredPersones.Where(p =>
                    (p.name != null && p.name.ToLower().Contains(searchTerm)) ||
                    (p.title != null && p.title.ToLower().Contains(searchTerm)) ||
                    (p.NationalNumber != null && p.NationalNumber.ToLower().Contains(searchTerm))
                );
            }

            // Search by ID
            if (searchId.HasValue)
            {
                filteredPersones = filteredPersones.Where(p => p.Id == searchId.Value);
            }

            // Filter by generation
            if (generation.HasValue)
            {
                filteredPersones = filteredPersones.Where(p => p.Generation == generation.Value);
            }

            var filteredList = filteredPersones.ToList();

            // Calculate pagination based on filtered results
            var totalCount = filteredList.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Get paginated data - ordered by Generation, then by DateOfBirth
            var paginatedPersones = filteredList
                .OrderBy(p => p.Generation)
                .ThenBy(p => p.Parent?.name ?? "")
                .ThenBy(p => p.DateOfBirth ?? DateTime.MaxValue)
                .ThenBy(p => p.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination and search info to view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.AllPersones = allPersones; // For statistics (all persons, not filtered)
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SearchId = searchId;
            ViewBag.Generation = generation;

            return View(paginatedPersones);
        }
        public ActionResult Root()
        {
            var GetPersone = db.persones.Include("children").ToList();
            var Root = GetPersone.FirstOrDefault(x => x.Id == 1);

            var r = JsonConvert.SerializeObject(Root,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Content(r, "application/json");
        }

        // API endpoint to get family tree for a specific person
        [HttpGet]
        public ActionResult GetFamilyTree(int id)
        {
            try
            {
                var targetPerson = db.persones.Include("Parent").FirstOrDefault(p => p.Id == id);
                if (targetPerson == null)
                {
                    return Json(new { success = false, message = "الشخص غير موجود" });
                }

                // Get all ancestors (going up the tree to the root)
                var ancestors = new List<object>();
                var currentPersonId = targetPerson.Parent?.Id;
                var processedIds = new HashSet<int>(); // Prevent infinite loops
                var maxDepth = 20; // Safety limit for very deep family trees
                var currentDepth = 0;

                while (currentPersonId.HasValue &&
                       !processedIds.Contains(currentPersonId.Value) &&
                       currentDepth < maxDepth)
                {
                    processedIds.Add(currentPersonId.Value);
                    currentDepth++;

                    var parent = db.persones.Include("Parent").FirstOrDefault(p => p.Id == currentPersonId.Value);
                    if (parent != null)
                    {
                        ancestors.Insert(0, new
                        {
                            Id = parent.Id,
                            name = parent.name,
                            title = parent.title,
                            image = parent.image,
                            Country = parent.Country,
                            DateOfBirth = parent.DateOfBirth,
                            DateOfDeath = parent.DateOfDeath,
                            Generation = parent.Generation
                        });

                        // Move to the next parent up the tree
                        currentPersonId = parent.Parent?.Id;

                        // If no more parents, we've reached the root
                        if (currentPersonId == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // Get all descendants (going down the tree)
                var descendants = GetDescendantsRecursive(targetPerson.Id);

                var result = new
                {
                    success = true,
                    data = new
                    {
                        ancestors = ancestors,
                        targetPerson = new
                        {
                            Id = targetPerson.Id,
                            name = targetPerson.name,
                            title = targetPerson.title,
                            image = targetPerson.image,
                            Country = targetPerson.Country,
                            DateOfBirth = targetPerson.DateOfBirth,
                            DateOfDeath = targetPerson.DateOfDeath,
                            Generation = targetPerson.Generation
                        },
                        descendants = descendants
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء جلب بيانات شجرة العائلة" });
            }
        }

        private List<object> GetDescendantsRecursive(int parentId, HashSet<int> processedIds = null)
        {
            // Initialize processedIds to prevent infinite loops
            if (processedIds == null)
                processedIds = new HashSet<int>();

            // Prevent infinite recursion
            if (processedIds.Contains(parentId))
                return new List<object>();

            processedIds.Add(parentId);

            var children = db.persones.Where(p => p.Parent != null && p.Parent.Id == parentId).ToList();

            return children.Select(child => new
            {
                person = new
                {
                    Id = child.Id,
                    name = child.name,
                    title = child.title,
                    image = child.image,
                    Country = child.Country,
                    DateOfBirth = child.DateOfBirth,
                    DateOfDeath = child.DateOfDeath,
                    Generation = child.Generation
                },
                children = GetDescendantsRecursive(child.Id, new HashSet<int>(processedIds)) // Pass a copy to avoid shared state
            }).Cast<object>().ToList();
        }

        public ActionResult Tree()
        {
            return View();
        }

        public ActionResult TreeOld()
        {
            return View("TreeOld");
        }

        public ActionResult IndexOld()
        {
            return View("TreeOld", db.persones.ToList());
        }

        //public ActionResult CreateArabic(int? id)
        //{
        //    if (id != null)
        //    {
        //        var GetParent = db.persones.Where(x => x.Id == id).ToList();
        //        ViewBag.GetParents = GetParent;
        //    }
        //    else
        //    {
        //        var GetParent = db.persones.ToList();
        //        ViewBag.GetParents = GetParent;
        //    }
             
        //    return View("CreateArabic");
        //}

        // GET: Persone/Details/5
        public ActionResult Details(int id)
        {
            var persone = db.persones.Include("Parent").Include("children").FirstOrDefault(p => p.Id == id);
            if (persone == null)
            {
                TempData["ErrorMessage"] = "الشخص المطلوب غير موجود.";
                return RedirectToAction(nameof(Index));
            }

            // Get all children
            var children = db.persones.Where(p => p.Parent != null && p.Parent.Id == id).ToList();
            ViewBag.Children = children;

            return View(persone);
        }

        // GET: Persone/Create
        public ActionResult Create(int? id)
        {
            if (id != null)
            {
                // Adding child to specific parent - include parent hierarchy
                var selectedParent = db.persones.Include("Parent").Where(x => x.Id == id).ToList();
                ViewBag.GetParents = selectedParent;
            }
            else
            {
                // General create - get all potential parents with their parent information
                var allPersones = db.persones.Include("Parent").ToList();
                ViewBag.GetParents = allPersones;
                
                // If no family members exist yet, this will be the root person
                if (!allPersones.Any())
                {
                    ViewBag.IsFirstPerson = true;
                }
            }
             
            return View();
        }

        // POST: Persone/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var createdPersons = new List<Persone>();

                // Check if this is multiple children submission
                var childNames = collection["ChildNames"].ToList();
                var childTitles = collection["ChildTitles"].ToList();
                var childImages = collection["ChildImages"].ToList();
                var childCountries = collection["ChildCountries"].ToList();
                var childPlacesOfRegistration = collection["ChildPlacesOfRegistration"].ToList();
                var childNationalNumbers = collection["ChildNationalNumbers"].ToList();
                var childDatesOfBirth = collection["ChildDatesOfBirth"].ToList();
                var childDatesOfDeath = collection["ChildDatesOfDeath"].ToList();

                if (childNames.Any() && childNames.Any(name => !string.IsNullOrWhiteSpace(name)))
                {
                    // Multiple children creation
                    Persone parent = null;

                    // Handle parent selection
                    if (!string.IsNullOrEmpty(collection["Parent"]) && int.TryParse(collection["Parent"], out int parentId))
                    {
                        parent = db.persones.Where(x => x.Id == parentId).FirstOrDefault();
                    }

                    if (parent == null)
                    {
                        ModelState.AddModelError("", "يجب اختيار الوالد لإضافة أطفال متعددين.");
                        ViewBag.GetParents = db.persones.ToList();
                        return View();
                    }

                    // First, create the main person (from top form fields) as the first child
                    if (!string.IsNullOrWhiteSpace(collection["name"]))
                    {
                        var mainPerson = new Persone
                        {
                            name = collection["name"],
                            title = collection["title"],
                            image = collection["image"],
                            Country = collection["Country"],
                            PlaceOfRegistration = collection["PlaceOfRegistration"],
                            NationalNumber = collection["NationalNumber"],
                            Parent = parent,
                            Generation = parent.Generation + 1,
                            DateOfBirth = null,
                            DateOfDeath = null
                        };

                        // Handle DateOfBirth for main person
                        if (!string.IsNullOrEmpty(collection["DateOfBirth"]))
                        {
                            if (DateTime.TryParse(collection["DateOfBirth"], out DateTime birthDate))
                            {
                                mainPerson.DateOfBirth = birthDate;
                            }
                        }

                        // Handle DateOfDeath for main person
                        if (!string.IsNullOrEmpty(collection["DateOfDeath"]))
                        {
                            if (DateTime.TryParse(collection["DateOfDeath"], out DateTime deathDate))
                            {
                                mainPerson.DateOfDeath = deathDate;
                            }
                        }

                        db.persones.Add(mainPerson);
                        createdPersons.Add(mainPerson);
                    }

                    // Then create additional children (siblings)
                    for (int i = 0; i < childNames.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(childNames[i]))
                            continue;

                        var child = new Persone
                        {
                            name = childNames[i],
                            title = i < childTitles.Count ? childTitles[i] : null,
                            image = i < childImages.Count ? childImages[i] : null,
                            Country = i < childCountries.Count ? childCountries[i] : null,
                            PlaceOfRegistration = i < childPlacesOfRegistration.Count ? childPlacesOfRegistration[i] : null,
                            NationalNumber = i < childNationalNumbers.Count ? childNationalNumbers[i] : null,
                            Parent = parent,
                            Generation = parent.Generation + 1,
                            DateOfBirth = null,
                            DateOfDeath = null
                        };

                        // Handle DateOfBirth for child
                        if (i < childDatesOfBirth.Count && !string.IsNullOrEmpty(childDatesOfBirth[i]))
                        {
                            if (DateTime.TryParse(childDatesOfBirth[i], out DateTime birthDate))
                            {
                                child.DateOfBirth = birthDate;
                            }
                        }

                        // Handle DateOfDeath for child
                        if (i < childDatesOfDeath.Count && !string.IsNullOrEmpty(childDatesOfDeath[i]))
                        {
                            if (DateTime.TryParse(childDatesOfDeath[i], out DateTime deathDate))
                            {
                                child.DateOfDeath = deathDate;
                            }
                        }

                        db.persones.Add(child);
                        createdPersons.Add(child);
                    }

                    if (createdPersons.Count > 0)
                    {
                        db.SaveChanges();
                        TempData["SuccessMessage"] = $"تم إضافة {createdPersons.Count} أطفال بنجاح للوالد {parent.name}";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "لم يتم إضافة أي أطفال. يرجى إدخال أسماء صحيحة.";
                    }
                }
                else
                {
                    // Single person creation (original logic)
                    Persone persone = new Persone();

                    persone.name = collection["name"];
                    persone.title = collection["title"];
                    persone.image = collection["image"];
                    persone.Country = collection["Country"];
                    persone.PlaceOfRegistration = collection["PlaceOfRegistration"];
                    persone.NationalNumber = collection["NationalNumber"];

                    // Handle DateOfBirth
                    if (!string.IsNullOrEmpty(collection["DateOfBirth"]))
                    {
                        if (DateTime.TryParse(collection["DateOfBirth"], out DateTime birthDate))
                        {
                            persone.DateOfBirth = birthDate;
                        }
                    }

                    // Handle DateOfDeath
                    if (!string.IsNullOrEmpty(collection["DateOfDeath"]))
                    {
                        if (DateTime.TryParse(collection["DateOfDeath"], out DateTime deathDate))
                        {
                            persone.DateOfDeath = deathDate;
                        }
                    }

                    // Handle parent selection
                    if (!string.IsNullOrEmpty(collection["Parent"]) && int.TryParse(collection["Parent"], out int parentId))
                    {
                        // Regular family member with parent
                        var parent = db.persones.Where(x => x.Id == parentId).FirstOrDefault();
                        if (parent != null)
                        {
                            persone.Parent = parent;
                            persone.Generation = parent.Generation + 1;
                        }
                        else
                        {
                            // Fallback if parent not found
                            persone.Parent = null;
                            persone.Generation = 1;
                        }
                    }
                    else
                    {
                        // Root person (no parent) - this is the family patriarch/matriarch
                        persone.Parent = null;
                        persone.Generation = 1;
                    }
                    
                    db.persones.Add(persone);
                    createdPersons.Add(persone);
                    db.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // In production, log the exception
                ModelState.AddModelError("", "حدث خطأ أثناء إضافة الفرد. يرجى المحاولة مرة أخرى.");
                
                // Reload parents for the view
                ViewBag.GetParents = db.persones.ToList();
                return View();
            }
        }

        // GET: Persone/Edit/5
        public ActionResult Edit(int id)
        {
            var persone = db.persones.Include("Parent").SingleOrDefault(p => p.Id==id);
            if (persone == null)
            {
                TempData["ErrorMessage"] = "الشخص المطلوب تعديله غير موجود.";
                return RedirectToAction(nameof(Index));
            }

            // Get all potential parents (exclude the person itself and their descendants to prevent circular references)
            var allPersones = db.persones.Include("Parent").ToList();
            var excludeIds = GetAllDescendantIds(id, allPersones);
            excludeIds.Add(id); // Also exclude the person itself

            var potentialParents = allPersones.Where(p => !excludeIds.Contains(p.Id)).ToList();
            ViewBag.GetParents = potentialParents;

            return View(persone);
        }

        private List<int> GetAllDescendantIds(int personId, List<Persone> allPersones)
        {
            var descendantIds = new List<int>();
            var children = allPersones.Where(p => p.Parent?.Id == personId).ToList();

            foreach (var child in children)
            {
                descendantIds.Add(child.Id);
                descendantIds.AddRange(GetAllDescendantIds(child.Id, allPersones));
            }

            return descendantIds;
        }

        // POST: Persone/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var persone = db.persones.Include("Parent").SingleOrDefault(p => p.Id == id);

                if (persone == null)
                {
                    TempData["ErrorMessage"] = "الشخص المطلوب تعديله غير موجود.";
                    return RedirectToAction(nameof(Index));
                }

                var originalParentId = persone.Parent?.Id;

                // Update basic information
                persone.name = collection["name"];
                persone.title = collection["title"];
                persone.image = collection["image"];
                persone.Country = collection["Country"];
                persone.PlaceOfRegistration = collection["PlaceOfRegistration"];
                persone.NationalNumber = collection["NationalNumber"];

                // Update DateOfBirth
                if (!string.IsNullOrEmpty(collection["DateOfBirth"]))
                {
                    if (DateTime.TryParse(collection["DateOfBirth"], out DateTime birthDate))
                    {
                        persone.DateOfBirth = birthDate;
                    }
                }
                else
                {
                    persone.DateOfBirth = null;
                }

                // Update DateOfDeath
                if (!string.IsNullOrEmpty(collection["DateOfDeath"]))
                {
                    if (DateTime.TryParse(collection["DateOfDeath"], out DateTime deathDate))
                    {
                        persone.DateOfDeath = deathDate;
                    }
                }
                else
                {
                    persone.DateOfDeath = null;
                }

                // Handle parent selection
                var newParentId = (int?)null;
                if (!string.IsNullOrEmpty(collection["Parent"]) && int.TryParse(collection["Parent"], out int parentIdValue))
                {
                    newParentId = parentIdValue;
                }

                // Check if parent changed
                if (originalParentId != newParentId)
                {
                    if (newParentId.HasValue)
                    {
                        var newParent = db.persones.SingleOrDefault(p => p.Id == newParentId.Value);
                        if (newParent != null)
                        {
                            // Verify this doesn't create a circular reference
                            var allPersones = db.persones.Include("Parent").ToList();
                            var excludeIds = GetAllDescendantIds(id, allPersones);
                            excludeIds.Add(id);

                            if (excludeIds.Contains(newParentId.Value))
                            {
                                TempData["ErrorMessage"] = "لا يمكن تعيين هذا الشخص كوالد لأنه سيؤدي إلى مرجع دائري في شجرة العائلة.";

                                // Reload parent data for the view
                                var potentialParents = allPersones.Where(p => !excludeIds.Contains(p.Id)).ToList();
                                ViewBag.GetParents = potentialParents;
                                return View(persone);
                            }

                            persone.Parent = newParent;
                            persone.Generation = newParent.Generation + 1;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "الوالد المحدد غير موجود.";

                            // Reload parent data for the view
                            var allPersones = db.persones.Include("Parent").ToList();
                            var excludeIds = GetAllDescendantIds(id, allPersones);
                            excludeIds.Add(id);
                            var potentialParents = allPersones.Where(p => !excludeIds.Contains(p.Id)).ToList();
                            ViewBag.GetParents = potentialParents;
                            return View(persone);
                        }
                    }
                    else
                    {
                        // Remove parent (make root)
                        persone.Parent = null;
                        persone.Generation = 1;
                    }

                    // Update generation for all descendants recursively
                    UpdateDescendantGenerations(persone.Id);
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = $"تم تحديث بيانات {persone.name} بنجاح.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ التعديلات. يرجى المحاولة مرة أخرى.";

                // Reload parent data for the view in case of error
                var allPersones = db.persones.Include("Parent").ToList();
                var excludeIds = GetAllDescendantIds(id, allPersones);
                excludeIds.Add(id);
                var potentialParents = allPersones.Where(p => !excludeIds.Contains(p.Id)).ToList();
                ViewBag.GetParents = potentialParents;
                return View();
            }
        }

        private void UpdateDescendantGenerations(int parentId)
        {
            var parent = db.persones.SingleOrDefault(p => p.Id == parentId);
            if (parent == null) return;

            var children = db.persones.Where(p => p.Parent != null && p.Parent.Id == parentId).ToList();
            foreach (var child in children)
            {
                child.Generation = parent.Generation + 1;
                UpdateDescendantGenerations(child.Id);
            }
        }

        // GET: Persone/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Persone/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var persone = db.persones.Include(p => p.children).FirstOrDefault(p => p.Id == id);

                if (persone == null)
                {
                    TempData["ErrorMessage"] = "الشخص المطلوب حذفه غير موجود.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete all children recursively
                DeletePersonAndChildren(persone);

                db.SaveChanges();

                TempData["SuccessMessage"] = $"تم حذف {persone.name} وجميع الأبناء المرتبطين بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء عملية الحذف. يرجى المحاولة مرة أخرى.";
                return RedirectToAction(nameof(Index));
            }
        }

        private void DeletePersonAndChildren(Persone persone)
        {
            // First, recursively delete all children
            var children = db.persones.Where(p => p.Parent.Id == persone.Id).ToList();
            foreach (var child in children)
            {
                DeletePersonAndChildren(child);
            }

            // Then delete the person
            db.persones.Remove(persone);
        }

    }
}