using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using PhotoSharingApplication.Models;
using PhotoSharingApplication.Data;
using System.Runtime.Remoting.Contexts;


namespace PhotoSharingApplication.Controllers
{
    public class PhotoController : Controller
    {

        private PhotoDBContext Context = new PhotoDBContext();

        // GET: Photo
        public ActionResult Index()
        {
            return View("Index");

        }

        [ChildActionOnly]
        public ActionResult _PhotoGallery(int number = 0)
        {
            List<Photo> photos;
            if (number == 0)
            {
                photos = Context.Photos.ToList();
            }
            else
            {
                photos = (
                from p in Context.Photos
                orderby p.CreatedDate descending
                select p).Take(number).ToList();
            }

            return PartialView("_PhotoGallery",photos);

        }


        public ActionResult Display(int id)
        {
            Photo photo =Context.Photos.Find(id);

            if (photo == null)
            {
                return HttpNotFound();
            }

            return View("Display", photo);


        }

        public ActionResult Create()
        {
            Photo newPhoto = new Photo();
            newPhoto.CreatedDate =DateTime.Today;
            return View("Create", newPhoto);

        }

        [HttpPost]
        public ActionResult Create(Photo photo, HttpPostedFileBase image)
        {
            photo.CreatedDate = DateTime.Today;

            if (!ModelState.IsValid)
            {
                return View("Create", photo);
            }
            else
            {
                if (image != null)
                {
                    photo.ImageMimeType =
                    image.ContentType;
                    photo.PhotoFile = new
                    byte[image.ContentLength];
                    image.InputStream.Read(
                    photo.PhotoFile, 0,
                    image.ContentLength);
                }
            }

            Context.Photos.Add(photo);
            Context.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            Photo photo =Context.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View("Delete", photo);

        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo =Context.Photos.Find(id);

            Context.Photos.Remove(photo);
            Context.SaveChanges();
            return RedirectToAction("Index");


        }

        public FileContentResult GetImage(int id)
        {
            Photo photo = Context.Photos.Find(id);


            if (photo != null)
            {
                return File(photo.PhotoFile, photo.ImageMimeType);
            }
            else
            {
                return null;
            }

        }




    }
}