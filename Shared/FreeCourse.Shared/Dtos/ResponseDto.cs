using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.Dtos
{
    public class ResponseDto<T> //Herhangi bir where ile kısıtlama yapmıyorum gelen herşey olabilir
    {//Servislerimin kullanacağı ortak alan
        public T Data { get; private set; }


        [JsonIgnore] // Ben yanlız kendi içinde kullanmak istiyorum. Yani bir daha Kodu göndermeme gerek yok (postman de sağ alttaki kod)
        public int StatusCode { get; private set; }

        [JsonIgnore] // Bunu da zaten gösteriyor kod içinde olsa bana yeter
        public bool IsSuccessful { get; private set; }//Başarılımı

        public List<string> Errors { get; set; }

        // Static Factory Method
        public static ResponseDto<T> Success(T data, int statusCode)
        {//Başarılı olup data alan
            return new ResponseDto<T>
            {
                Data = data,
                StatusCode = statusCode,
                IsSuccessful = true
            };
        }

        public static ResponseDto<T> Success(int statusCode)
        {
            return new ResponseDto<T>
            {
                Data = default(T),
                StatusCode = statusCode,
                IsSuccessful = true
            };
        }

        public static ResponseDto<T> Fail(List<string> errors, int statusCode)
        {//Birden fazla hata
            return new ResponseDto<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

        public static ResponseDto<T> Fail(string error, int statusCode)
        {
            return new ResponseDto<T>
            {
                Errors = new List<string>() { error },
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

    }
}
