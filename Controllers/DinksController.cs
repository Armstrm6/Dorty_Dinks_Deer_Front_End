using DinksCodeChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using DinksCodeChallenge.Models.ViewModel;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
namespace DinksCodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DinksController : Controller
    {
        private readonly CodeChallengeContext _context;
        public DinksController(CodeChallengeContext context)
        {
            _context = context;
        }

        // RUN CALLS SYNCRONOUSLY ON THE FRONT AND BACK END SO THAT YOU DONT RUN INTO RACE CONDITIONS.
        //PICTURE
        [HttpGet("GetPicture")]
        public FileStreamResult GetPictureInformation([FromQuery] int pictureId)
        {
                // processing the stream.
                //var photos = _context.DinkPictures.Where(element => element.PictureId == pictureId).FirstOrDefault();
                var photos = _context.DinkPictures.FirstOrDefault(element => element.PictureId == pictureId);
                // firstorDefault is a safetynet that will return a NULL object if nothing is found in the Database.
                var stream = new MemoryStream(photos.Picture);
                return new FileStreamResult(stream, "image/jpeg");

        }


        //CREATE
        [HttpPost("CreateDinks")]
        [Consumes("multipart/form-data")]
        public  ActionResult CreateDinks([FromForm] double geoX, [FromForm] double geoY, [FromForm] DateTime dateNow, [FromForm] List<IFormFile> inputPhotos)
        {
            if (geoX == null || geoY == null || dateNow == null)
            {
                return NoContent();
            }

            // Format Date Properly
            DinkData Dink = new DinkData()
            {
                GeoX = geoX,
                GeoY = geoY,
                DeerDate = dateNow,
            };
            _context.DinkData.Add(Dink);
            _context.SaveChanges();
            foreach (IFormFile photo in inputPhotos)
            {
                DinkPictures picture = new DinkPictures();
                picture.DeerId = Dink.DeerId;
                // ORIGINAL METHOD OF CONVERTING IMG TO BYTES //
                // WITHOUT THE USE OF SYSTEM.DRAWING CLASSES  //
                if (photo.ContentType.ToLower().StartsWith("image/"))
                // Check whether the selected file is image
                {

                    byte[] b;
                    using (BinaryReader br = new BinaryReader(photo.OpenReadStream()))
                    {
                        b = br.ReadBytes((int)photo.OpenReadStream().Length);
                        // Convert the image in to bytes
                        picture.Picture = b;
                    }
                    _context.DinkPictures.Add(picture);
                    _context.SaveChanges();
                }

            }
            return Ok();
        }

        //
        //READ ALL
        [HttpGet("GetAllDinksInformation")]
        public  ActionResult<DinkData> GetAllInformation()
        {

            var data =  _context.DinkData.Where(element => element.DeerId > 0)
                .Include(element => element.DinkPictures)
                // Select(new) is a runtime object. Alows you to define the varibles of the JSon object
                .Select(s => new { randomName = s.DeerDate, s.DeerId, s.GeoX, s.GeoY,  PictureId = s.DinkPictures.Select(s => s.PictureId)})
                .ToList();

            return Ok(data);
        }
        

        //READ ONE
        [HttpGet("GetSingleDink/{DeerID}")]
        public  ActionResult GetSingleDink([FromQuery] int? DeerID)
        {
            if (DeerID == null)
            {
                return NoContent();
            }
            if (DeerID == null)
            {
                return NoContent();
            }
            var singleDink =  _context.DinkData.Where(element => element.DeerId == DeerID)
                .Include(element => element.DinkPictures)
                .Select(s => new { s.DeerDate, s.DeerId, s.GeoX, s.GeoY, PictureId = s.DinkPictures.Select(s => s.PictureId) })
                .First();
            return Ok(singleDink);
        }

        
        //UPDATE
        [HttpPost("UpdateDinks")]
        [Consumes("multipart/form-data")]
        public  ActionResult UpdateDinks([FromForm] int DeerID, [FromForm] double geoX, [FromForm] double geoY, [FromForm] DateTime dateNow, [FromForm] List<IFormFile> inputPhotos)
        {
            if (DeerID == null || geoX == null || geoY == null || dateNow == null)
            {
                return NoContent();
            }

            var data = new DinkData()
            {
                DeerId = DeerID,
                GeoX = geoX,
                GeoY = geoY,
                DeerDate = dateNow
            };
            _context.DinkData.Update(data);
            _context.SaveChanges();

            foreach (IFormFile photo in inputPhotos)
            {
                DinkPictures picture = new DinkPictures();
                picture.DeerId = data.DeerId;
                // ORIGINAL METHOD OF CONVERTING IMG TO BYTES //
                // WITHOUT THE USE OF SYSTEM.DRAWING CLASSES  //
                if (photo.ContentType.ToLower().StartsWith("image/"))
                // Check whether the selected file is image
                {

                    byte[] b;
                    using (BinaryReader br = new BinaryReader(photo.OpenReadStream()))
                    {
                        b = br.ReadBytes((int)photo.OpenReadStream().Length);
                        // Convert the image in to bytes
                        picture.Picture = b;
                    }
                    _context.DinkPictures.Add(picture);
                    _context.SaveChanges();
                }

            }
            return Ok();
        }


        //DELETE ALL
        [HttpDelete("DeleteDinks/{DeerId}")]
        public  ActionResult DeleteDinks([FromQuery] int deerId)
        {
            if (deerId == null)
            {
                return NoContent();
            }

            // Method Regular Delete.
            var dinkToRemove =  _context.DinkData.Where(element => element.DeerId == deerId).First();
            var photoToRemove =  _context.DinkPictures.AsQueryable().Where(element => element.DeerId == deerId).ToList();

            //check and see if we have records      
            if(photoToRemove.Count > 0)
            {
                // saves time in the compiler
                _context.DinkPictures.RemoveRange(photoToRemove);
                _context.SaveChanges();
            }

            foreach (var photo in photoToRemove)
            {
                _context.DinkPictures.Remove(photo);
                _context.SaveChanges();
            }

            _context.DinkData.Remove(dinkToRemove);
            _context.SaveChanges();
            return Ok();
        }

        //DELETE ONE
        [HttpDelete("DeletePicture")]
        public  ActionResult DeleteSinglePhoto([FromQuery] int pictureId)
        {
            var singlePhoto = _context.DinkPictures.Where(element => element.PictureId == pictureId).FirstOrDefault();

            _context.DinkPictures.Remove(singlePhoto);
            _context.SaveChanges();
            return Ok();
        }
    }
}
