using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {//CustomBaseController kalıtım alındı

        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
        {
            //CancellationToken alma amacım diyelimki buraya bir fotoğraf geldiğinde farz edelimki 20 saniye sürüyor photo yükleme işlemi işlemi sonlandırınca buradaki fotoğraf kaydetme işlemide sonlansın devam etmesin. CancellationToken'a parametre göndermicem otomatik bir şekilde tetikleniyor olacak. async bir metodu sadece hata fırlatarak sonralandırabiliriz.

            if (photo != null && photo.Length > 0)
            {
                //dosya ismini buradan random vermiyorum bu metodu çalıştırırken yolluyor olacağım
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {//ilgili scope bitince bu stream direk bellekten düşecek
                    //yukarıya gelen fotoğrafı kopyalıyorum
                    await photo.CopyToAsync(stream, cancellationToken);//olurda kullanıcıyı tarayıcıyı kapatırsa veya istek yarıda kesilirse hata vericek işlem sonlanacak
                }

                //fotoğraf kaydedilten sonra nasıl bir path dönicem onu belirlicem
                var returnPath = "photos/" + photo.FileName;

                PhotoDto photoDto = new() { Url = returnPath };

                return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
            }

            return CreateActionResultInstance(Response<PhotoDto>.Fail("photo is empty", 400));//fotoğraf boş hatası

        }
    }
}
