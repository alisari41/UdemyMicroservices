using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Services
{
    public class RedisService
    {// Redis İle Bağlantı kurmaktan sorumlu sınıf
        private readonly string _host;
        private readonly int _port;

        // Redisle bağlantı kurabilme için tanımladım
        private ConnectionMultiplexer _ConnectionMultiplexer;


        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        //bağlantı kuruluyor
        public void Connect() => _ConnectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");

        // Birde bana Redis hazır veritabanı versin
        public IDatabase GetDb(int db = 1) => _ConnectionMultiplexer.GetDatabase(db);
    }
}
