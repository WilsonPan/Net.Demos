using System;
using AutoMapper;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dto, Entity>()
                   .ForMember(dest => dest.CreateTime,
                              opt => opt.MapFrom(src => new DateTime(DateTime.Now.Ticks)));
            });
            var mapper = configuration.CreateMapper();

            var entity = mapper.Map<Entity>(new Dto
            {
                Id = 1,
                Name = "Wilson",
                Hour = 12,
                Minute = 12,
                Second = 12
            });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(entity));

            Console.ReadKey(true);
        }
    }


    class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class Dto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }
}
