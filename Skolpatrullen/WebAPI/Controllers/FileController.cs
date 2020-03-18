﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.ViewModels;

namespace WebAPI.Controllers
{
    public class FileController : APIController
    {
        public FileController(Context context, ILogger<UserController> logger) : base(context, logger)
        {
        }

        [HttpGet]
        [Route("[controller]/GetFileById/{Id}")]
        public APIResponse<File> GetFileById(int Id)
        {
            APIResponse<File> response = new APIResponse<File>();
            response.Data = _context.Files.SingleOrDefault(c => c.Id == Id);

            response.Success = true;
            response.SuccessMessage = $"Hämtade fil med id {Id}";
            return response;
        }

        [HttpGet]
        [Route("[controller]/DeleteFileById/{Id}")]
        public APIResponse<File> DeleteFileById(int Id)
        {
            APIResponse<File> response = new APIResponse<File>();
            response.Data = _context.Files.SingleOrDefault(c => c.Id == Id);

            if (response.Data != null)
            {
                _context.Remove(response.Data);
                _context.SaveChanges();
                response.SuccessMessage = $"Tog bort fil med id {Id}";
            }
            else
            {
                response.SuccessMessage = $"Det fanns ingen fil med id {Id}";
            }
            response.Success = true;


            return response;

        }
        [HttpPost]
        [Route("[controller]/UploadCourseFile")]
        public APIResponse UploadCourseFile(FileBody body)
        {
            APIResponse response = new APIResponse();
            User user = _context.Users.SingleOrDefault(u => u.Id == body.UserId);

            if (user != null && body.File.Length > 0)
            {
                File file = new File();
                CourseFile coursefile = new CourseFile();

                file.Binary = body.File;
                file.UploadDate = body.UploadDate;
                file.FileExtension = body.FileExtension;

                _context.Files.Add(file);
                _context.SaveChanges();

                coursefile.CourseId = body.CourseId;
                coursefile.FileId = file.Id;
                coursefile.Name = file.FileExtension;

                _context.CourseFiles.Add(coursefile);
                _context.SaveChanges();

                response.Success = true;
            }
            else
            {
                response.FailureMessage = "Filen laddades inte upp";
                response.Success = false;
            }

            return response;
        }

    }
}
