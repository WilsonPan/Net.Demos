using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            Mapper.Initialize(cfg =>
            {
                cfg.AddMemberConfiguration();

                cfg.CreateMap<Dto, Entity>()
                   .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => new DateTime(DateTime.Now.Ticks)));
            });
            try
            {
                var entity = Mapper.Map<Dto, Entity>(new Dto { Id = 1, Name = "Wilson", Hour = 12, Minute = 12, Second = 12 });

                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(entity));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
