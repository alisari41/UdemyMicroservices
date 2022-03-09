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
    internal class CategoryService : ICategoryService
    {//Veritanındaki verileri okuyup Dtolara atama işlemi
        private readonly IMongoCollection<Category> _categoryCollection;

        //Dönüştürme işlemi 
        private readonly IMapper _mapper;

        //Bunları doldurmak için IDatabaseSettings kullanıyorum
        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {//Erişebilirle hata verirse class 'ı internal tanımla
            //veri tabanına Server'a bağlanmam lazım 
            var client = new MongoClient(databaseSettings.ConnectionString);

            //veri tabanına bağlanabiliriz
            var database = client.GetDatabase(databaseSettings.DatabaseName);


            // _categoryCollection işlemini artık doldurabiliriz Category CategoryCollectionName leri diğer sınıflarda düzelt
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        
        // Listeleme
        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryCollection.Find(category => true).ToListAsync();//Tüm categoryleri bana ver

            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);
        }

      
        // Ekleme 
        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {//CreateCategoryDto oluşturursam buraya parametre olarak verebilirim.

            var category = _mapper.Map<Category>(categoryDto);// CategoryDto nesnesini Category'e dönüştürme işlemi
            await _categoryCollection.InsertOneAsync(category);

            //                                   Create işleminde dataları yollamamız lazım
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }


        // Id'e göre category listeleme
        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();

            if (category == null)
            {//Eğer girilen id'e ait category bulunamazsa
                return Response<CategoryDto>.Fail("Category not found", 404);
            }

            //                                   Listeleme işleminde dataları yollamamız lazım
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }


    }
}
