using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using fileManagerApi.Models;
using fileManagerApi.ViewModel;

namespace fileManagerApi.Controllers
{
    public class ServiceController : ApiController
    {
        DB01Entities2 db = new DB01Entities2();
        resultModel result = new resultModel();
        #region Folders
        [HttpGet]
        [Route("api/getFolderList")]
        public List<folderModel> FolderList()
        {
            List<folderModel> folder = db.folders.Select(x => new folderModel()
            {
                folderId = x.folderId,
                folderOwnerId = x.folderOwnerId,
                folderName = x.folderName,
                folderCreateTime = x.folderCreateTime,
                folderGroupLevel = x.folderGroupLevel,
            })
            .ToList();
            return folder;
        }

        [HttpGet]
        [Route("api/getFolderById/{folderId}")]
        public folderModel folderById(string folderId)
        {
            folderModel folder = db.folders.Where(x => x.folderId == folderId).Select(x => new
            folderModel()
            {
                folderId = x.folderId,
                folderOwnerId = x.folderOwnerId,
                folderName = x.folderName,
                folderCreateTime = x.folderCreateTime,
                folderGroupLevel = x.folderGroupLevel,
            }).SingleOrDefault();
            return folder;
        }

        [HttpPost]
        [Route("api/addFolder")]
        public resultModel addFolder(folderModel model)
        {
            if (db.folders.Count(x => x.folderName == model.folderName) > 0)
            {
                result.proccess = false;
                result.message = "Girilen Klasör ismi zaten mevcut";
                return result;
            }
            folders newFolder = new folders();
            newFolder.folderId = Guid.NewGuid().ToString();
            newFolder.folderName = model.folderName;
            newFolder.folderGroupLevel = model.folderGroupLevel;
            newFolder.folderOwnerId = model.folderOwnerId;
            db.folders.Add(newFolder);
            db.SaveChanges();
            result.proccess = true;
            result.message = "Klasör oluşturuldu";
            return result;
        }

        [HttpPut]
        [Route("api/editFolder")]
        public resultModel editFolder(folderModel model)
        {
            folders register = db.folders.Where(x => x.folderId == model.folderId).FirstOrDefault();
            int nameCount = db.folders.Count(x => x.folderName == model.folderName);
            if (register == null)
            {
                result.proccess = false;
                result.message = "Geçerli bir klasör numarası giriniz";
                return result;
            }
            if (nameCount > 0)
            {
                result.proccess = false;
                result.message = "Girilen Klasör ismi zaten mevcut";
                return result;
            }
            register.folderName = model.folderName;
            register.folderGroupLevel = model.folderGroupLevel;
            db.SaveChanges();
            result.proccess = true;
            result.message = "Dosya düzenlendi";
            return result;
        }


        [HttpDelete]
        [Route("api/deleteFolder/{folderId}")]
        public resultModel deleteFolder(string folderId)
        {
            folders folder = db.folders.Where(x => x.folderId == folderId).FirstOrDefault();

            if (db.files.Count(s => s.inFolderId == folderId) > 0)
            {
                result.proccess = false;
                result.message = "İçersinde dosya olan bir klasörü silemezsiniz";
                return result;
            }

            if (folder != null)
            {
                if (db.files.Count(s => s.inFolderId == folderId) <= 0)
                {
                    db.folders.Remove(folder);
                    db.SaveChanges();
                    result.proccess = true;
                    result.message = "Klasör başarıyla silindi";
                    return result;
                }
                result.proccess = false;
                result.message = "İçerisinde dosya bulunan klasörü silemezsiniz";
                return result;
            }
            else
            {
                result.proccess = false;
                result.message = "Belirtilen klasör bulunamadı";
                return result;
            }
        }

        [HttpGet]
        [Route("api/getFolderInFiles/{folderId}")]
        public int getFolderInFiles(string folderId)
        {
            int fileCount = db.files.Count(s => s.inFolderId == folderId);
            return fileCount;
        }

        #endregion

        #region users
        [HttpGet]
        [Route("api/getUserList")]
        public List<userModel> getUserList()
        {
            List<userModel> users = db.users.Select(s => new userModel()
            {
                userId = s.userId,
                userGroup = s.userGroup,
                userMail = s.userMail,
                userName = s.userName,
                userProfilImg = s.userProfilImg,
                userPassword = "************",
            }).ToList();
            return users;
        }

        [HttpGet]
        [Route("api/getUserById/{userId}")]
        public userModel getgetUserById(string userId)
        {
            userModel users = db.users.Where(s => s.userId == userId).Select(s => new userModel()
            {
                userId = s.userId,
                userGroup = s.userGroup,
                userMail = s.userMail,
                userName = s.userName,
                userProfilImg = s.userProfilImg,
                userPassword = "************",
            }).SingleOrDefault();
            return users;
        }

        [HttpPost]
        [Route("api/addUser")]
        public resultModel addUser(userModel model)
        {
            if (db.users.Count(s => s.userMail == model.userMail) > 0)
            {
                result.proccess = false;
                result.message = "Bu mail adresi zaten kullanılmakta";
                return result;
            }
            users newUser = new users();
            newUser.userId = Guid.NewGuid().ToString();
            newUser.userGroup = "user";
            newUser.userMail = model.userMail;
            newUser.userName = model.userName;
            newUser.userPassword = model.userPassword;
            newUser.userProfilImg = model.userProfilImg;
            db.users.Add(newUser);
            db.SaveChanges();
            result.proccess = true;
            result.message = "Kayıt başarıyla tamamlandı";
            return result;
        }

        [HttpPut]
        [Route("api/editUser")]
        public resultModel editUser(userModel model)
        {
            users register = db.users.Where(x => x.userId == model.userId).FirstOrDefault();
            int mailCount = db.users.Count(x => x.userMail == model.userMail);
            if (register == null)
            {
                result.proccess = false;
                result.message = "Kullanıcı bulunamadı";
                return result;
            }

            if (mailCount > 0)
            {
                if (model.userMail == register.userMail)
                {
                    result.proccess = false;
                    result.message = "Girmiş olduğun mail mevcut mail adresin ile aynı.";
                    return result;
                }
                result.proccess = false;
                result.message = "Girilen mail adresi zaten mevcut";
                return result;
            }

            if (model.userPassword != null)
            {
                if (model.userPassword == register.userPassword)
                {
                    result.proccess = false;
                    result.message = "Yeni şifren eski şifren ile aynı olamaz";
                    return result;
                }
                register.userPassword = model.userPassword;
            }

            if (model.userName == register.userName)
            {
                result.proccess = false;
                result.message = "Girmiş olduğunuz kullanıcı adı eski kullanıcı adınız ile aynı";
                return result;
            }

            if (model.userName == null)
                model.userName = register.userName;

            if (model.userMail == null)
                model.userMail = register.userMail;

            register.userName = model.userName;
            register.userMail = model.userMail;
            //register.userGroup = model.userGroup; // Yanlızca yöneticilerin erişebileceği bir bölümde sadece yetkilendirme için ayrı merhod yazılacak.
            db.SaveChanges();
            result.proccess = true;
            result.message = "Kullanıcı bilgileri Güncellendi";
            return result;
        }

        [HttpDelete]
        [Route("api/deleteUser/{userId}")]
        // Authorize yapılacak , sadece yetkililer erişebilecek.
        public resultModel deleteUser(string userId)
        {
            users user = db.users.Where(x => x.userId == userId).FirstOrDefault();

            if (db.folders.Count(s => s.folderOwnerId == userId) > 0)
            {
                result.proccess = false;
                result.message = "Klasör oluşturmuş bir kullanıcı verisi silinemez";// Bu durumda klasörleride silinecek şekilde ayarlanacak.
                return result;
            }

            if (user != null)
            {
                db.users.Remove(user);
                db.SaveChanges();
                result.proccess = true;
                result.message = "Üye kaydı başarıyla silindi";
                return result;
            }
            result.proccess = false;
            result.message = "Üye kaydı bulunamadı";
            return result;
        }
        #endregion

        #region Files
        [HttpGet]
        [Route("api/getFileList")]
        public List<fileModel> getFileList()
        {
            {
                List<fileModel> files = db.files.Select(f => new fileModel()
                {
                    fileDownloadCount = f.fileDownloadCount,
                    fileId = f.fileId,
                    fileName = f.fileName,
                    fileOwnerId = f.fileOwnerId,
                    fileType = f.fileType,
                    fileUpploadTime = f.fileUpploadTime,
                    fileUrl = f.fileUrl,
                    inFolderId = f.inFolderId
                }).ToList();
                return files;
            }
        }

        [HttpGet]
        [Route("api/getFileById/{fileId}")]
        public fileModel getFileById(string fileId)
        {
            fileModel file = db.files.Where(f => f.fileId == fileId).Select(x => new fileModel()
            {
                fileDownloadCount = x.fileDownloadCount,
                fileId = x.fileId,
                fileName = x.fileName,
                fileOwnerId = x.fileOwnerId,
                fileType = x.fileType,
                fileUpploadTime = x.fileUpploadTime,
                fileUrl = x.fileUrl,
                inFolderId = x.inFolderId
            }).SingleOrDefault();
            return file;
        }

        [HttpGet]
        [Route("api/getFolderInFile/{folderId}")]
        public List<fileModel> getFolderInFile(string folderId)
        {
            List<fileModel> files = db.files.Where(f => f.inFolderId == folderId).Select(s => new fileModel()
            {
                fileDownloadCount = s.fileDownloadCount,
                fileId = s.fileId,
                fileName = s.fileName,
                fileOwnerId = s.fileOwnerId,
                fileType = s.fileType,
                fileUpploadTime = s.fileUpploadTime,
                fileUrl = s.fileUrl,
                inFolderId = s.inFolderId
            }).ToList();
            return files;
        }

        [HttpPost]
        [Route("api/addFile")]
        public resultModel addFile(fileModel model)
        {
            if (db.files.Count(s => s.fileName == model.fileName && s.inFolderId == model.inFolderId) > 0)
            {
                result.proccess = false;
                result.message = "Bu klasörde bu isimde bir dosya zaten mevcut";
                return result;
            }
            else if (db.files.Count(s => s.fileUrl == model.fileUrl && s.inFolderId == model.inFolderId) > 0)
            {
                result.proccess = false;
                result.message = "Bu dosya bu klasör içinde zaten mevcut";
                return result;
            }
            DateTime now = DateTime.Now;
            files newFile = new files();
            newFile.fileDownloadCount = 0;
            newFile.fileId = Guid.NewGuid().ToString();
            newFile.fileName = model.fileName;
            newFile.fileOwnerId = model.fileOwnerId;
            newFile.fileType = model.fileType;
            newFile.fileUpploadTime = now.ToString("F");
            newFile.fileUrl = model.fileUrl;
            newFile.inFolderId = model.inFolderId;
            db.files.Add(newFile);
            db.SaveChanges();
            result.proccess = true;
            result.message = "Dosya başarıyla eklendi";
            return result;
        }

        [HttpPut]
        [Route("api/editFile")]
        public resultModel editFile(fileModel model)
        {
            files file = db.files.Where(x => x.fileId == model.fileId).FirstOrDefault();
            if (file != null)
            {
                file.fileName = model.fileName;
                result.proccess = true;
                result.message = "Dosya ismi başarıyla değiştirildi";
                return result;
            }
            result.proccess = false;
            result.message = "Bir hata oluştu , belirtilen dosya bulunamadı";
            return result;
        }

        [HttpDelete]
        [Route("api/deleteFile/{fileId}")]
        // authorize eklenecek. [Authorize]
        public resultModel deleteFile(string fileId)
        {
            files file = db.files.Where(x => x.fileId == fileId).FirstOrDefault();
            if (file == null)
            {
                result.proccess = false;
                result.message = "Silincek dosya bulunamadı";
                return result;
            }
            db.files.Remove(file);
            db.SaveChanges();
            result.proccess = true;
            result.message = "Dosya başarıyla silindi";
            return result;
        }

        [HttpPost]
        [Route("api/saveFile")]
        public resultModel saveFile(saveFileModel model)
        {
            string data = model.fileData;
            string base64 = data.Substring(data.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] fileBytes = Convert.FromBase64String(base64);
            string fileName = model.ownerId + model.fileUzanti.Replace("image/", ".");// uzantının ingilizcesini unuttum.
            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                Image img = Image.FromStream(ms, true);
                img.Save(System.Web.Hosting.HostingEnvironment.MapPath("~/Dosyalar/" + fileName));
            }
            db.SaveChanges();
            result.proccess = true;
            result.message = "Dosya yüklendi.";
            return result;
        }

        #endregion
    }
}
