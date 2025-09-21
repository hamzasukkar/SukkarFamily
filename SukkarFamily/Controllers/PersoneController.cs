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
        public ActionResult Index()
        {

            return View(db.persones.ToList());
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

                // Get all ancestors (going up the tree)
                var ancestors = new List<object>();
                var current = targetPerson;
                while (current?.Parent != null)
                {
                    var parent = db.persones.FirstOrDefault(p => p.Id == current.Parent.Id);
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
                        current = parent;
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

        private List<object> GetDescendantsRecursive(int parentId)
        {
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
                children = GetDescendantsRecursive(child.Id)
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
            return View();
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

                    // Create children
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
                            Parent = parent,
                            Generation = parent.Generation + 1,
                            DateOfBirth = null, // Can be set later through Edit
                            DateOfDeath = null
                        };

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
            var persone = db.persones.SingleOrDefault(p => p.Id==id);
            return View(persone);
        }

        // POST: Persone/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var persone = db.persones.SingleOrDefault(p => p.Id == id);

                if (persone == null)
                {
                    TempData["ErrorMessage"] = "الشخص المطلوب تعديله غير موجود.";
                    return RedirectToAction(nameof(Index));
                }

                // Update basic information
                persone.name = collection["name"];
                persone.title = collection["title"];
                persone.image = collection["image"];
                persone.Country = collection["Country"];

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

                db.SaveChanges();
                TempData["SuccessMessage"] = $"تم تحديث بيانات {persone.name} بنجاح.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حفظ التعديلات. يرجى المحاولة مرة أخرى.";
                return View();
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

                TempData["SuccessMessage"] = $"تم حذف {persone.name} وجميع الأطفال المرتبطين بنجاح.";
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