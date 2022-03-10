using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {//Veritanındaki verileri okuyup Dtolara atama işlemi
        private readonly IMongoCollection<Course> _courseCollection;

        private readonly IMongoCollection<Category> _categoryCollection;

        //Dönüştürme işlemi 
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {//Erişebilirle hata verirse class 'ı internal tanımla
            //veri tabanına Server'a bağlanmam lazım 
            var client = new MongoClient(databaseSettings.ConnectionString);

            //veri tabanına bağlanabiliriz
            var database = client.GetDatabase(databaseSettings.DatabaseName);


            // _courseCollection işlemini artık doldurabiliriz
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            // categoryleri doldurmak için oluşturuyorum
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;


        }


        // Kursları Listeleme
        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            //MongoDb NoSql veritabanlarında join işlemleri yok kullanılmaz

            var courses = await _courseCollection.Find(course => true).ToListAsync();


            if (courses.Any())
            {//kurs varsa
                foreach (var course in courses)
                {
                    // Categorysini doldur
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {//kurs yoksa
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }


        // Category Course id ile arama
        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }

            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }


        //User'ın kurslarını arama
        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            if (courses.Any())
            {//kurs varsa
                foreach (var course in courses)
                {
                    // Categorysini doldur
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {//kurs yoksa
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }


        // Kurs ekleme işlemi
        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);

            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }


        // Kurs Update. Güncelleme işleminde data dönmeme gerek yok o yüzden NoContent kullanıyorum
        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            //                                                                eğer  bulursan updateCourse u güncelle
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);

            if (result == null)
            {
                //Update metodlarında geriye data dönmüyoruz NoContent
                return Response<NoContent>.Fail("Course not found", 404);
            }
            //                                204 body'si olmayan başarılı bir durum kodunu ifade eder
            return Response<NoContent>.Success(204);
        }


        //Kurs Delete
        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount > 0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}
