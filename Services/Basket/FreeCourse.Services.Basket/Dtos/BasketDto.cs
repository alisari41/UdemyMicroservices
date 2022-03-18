using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string UserId { get; set; }//Sepet kime ait  
        public string DiscountCode { get; set; }//İndirim Kodu
        public List<BasketItemDto> basketItems { get; set; }

        public decimal TotalPrice
        {
            get => basketItems.Sum(x => x.Price * x.Quantity);
        }
    }
}
