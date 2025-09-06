using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var news = db.News.Find(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(News news, IFormFile ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image upload
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Validate file type
                        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                        if (!allowedTypes.Contains(ImageFile.ContentType.ToLower()))
                        {
                            ModelState.AddModelError("ImageFile", "يرجى اختيار ملف صورة صحيح (JPG, PNG, GIF, WebP)");
                            return View(news);
                        }

                        // Validate file size (5MB max)
                        const int maxFileSize = 5 * 1024 * 1024; // 5MB
                        if (ImageFile.Length > maxFileSize)
                        {
                            ModelState.AddModelError("ImageFile", "حجم الملف كبير جداً. الحد الأقصى 5MB");
                            return View(news);
                        }

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        // Create unique filename with timestamp and GUID
                        var fileExtension = Path.GetExtension(ImageFile.FileName);
                        var uniqueFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{fileExtension}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        // Save file asynchronously
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        
                        news.ImgUrl = "/uploads/news/" + uniqueFileName;
                    }
                    
                    // Set creation date
                    news.Date = DateTime.Now;
                    
                    db.News.Add(news);
                    db.SaveChanges();
                    
                    // Redirect to appropriate index based on current culture/language
                    return RedirectToAction("IndexArabic");
                }
                return View(news);
            }
            catch (Exception)
            {
                // Log error (in production, use proper logging)
                ModelState.AddModelError("", "حدث خطأ أثناء حفظ الخبر. يرجى المحاولة مرة أخرى.");
                return View(news);
            }
        }

        // GET: News/Edit/5
        public ActionResult Edit(int id)
        {
            var news = db.News.Find(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, News news, IFormFile imageFile)
        {
            try
            {
                if (id != news.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingNews = db.News.Find(id);
                    if (existingNews == null)
                    {
                        return NotFound();
                    }

                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(fileStream);
                        }
                        
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingNews.ImgUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingNews.ImgUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        
                        existingNews.ImgUrl = "/uploads/news/" + uniqueFileName;
                    }
                    
                    existingNews.Title = news.Title;
                    existingNews.Text = news.Text;
                    existingNews.Date = news.Date ?? existingNews.Date;
                    
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(news);
            }
            catch
            {
                return View(news);
            }
        }

        // GET: News/Delete/5
        public ActionResult Delete(int id)
        {
            var news = db.News.Find(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var news = db.News.Find(id);
                if (news != null)
                {
                    // Delete associated image file if exists
                    if (!string.IsNullOrEmpty(news.ImgUrl))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", news.ImgUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    
                    db.News.Remove(news);
                    db.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Arabic versions of the actions
        public ActionResult IndexArabic()
        {
            return View("~/Views/News/IndexArabic.cshtml", db.News.ToList());
        }
    }
}