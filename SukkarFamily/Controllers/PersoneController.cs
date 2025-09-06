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
                // Adding child to specific parent
                var selectedParent = db.persones.Where(x => x.Id == id).ToList();
                ViewBag.GetParents = selectedParent;
            }
            else
            {
                // General create - get all potential parents
                var allPersones = db.persones.ToList();
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
                Persone persone = new Persone();

                persone.name = collection["name"];
                persone.title = collection["title"];
                persone.image = collection["image"];
                
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
                db.SaveChanges();

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
                // TODO: Add update logic here
                var persone = db.persones.SingleOrDefault(p => p.Id == id);
                persone.name = collection["name"];
                persone.title= collection["title"];
                persone.image= collection["image"];
                //persone.Parent = db.persones.SingleOrDefault(p => p.Id == Int32.Parse(collection["Parent"]));
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
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
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}