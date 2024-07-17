using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SukkarFamily.Models;

namespace SukkarFamily.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        private DB db;
        public NewsController(DB db)
        {
            this.db = db;
        }
        public ActionResult Index()
        {
            
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                News news = new News();
                news.Text = collection["Text"];
                news.Title = collection["Title"];
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: News/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: News/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: News/Delete/5
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