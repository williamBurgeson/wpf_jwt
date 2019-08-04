using Server.Infrastructure.Services;
using Newtonsoft.Json;
using SharedModels.Models;
using System;

namespace UserGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
                throw new NotImplementedException("This tool does not yet support command line args");

            var loginModel = new LoginModel();

            Console.WriteLine("Please enter the username: ");

            loginModel.Username = Console.ReadLine();

            Console.WriteLine($"Username entered: {loginModel.Username}");

            Console.WriteLine("Please enter the password: ");

            loginModel.Password = Console.ReadLine();

            Console.WriteLine($"Password entered: {loginModel.Password}");

            var userEntity = new SecurityService(null).GenerateUserEntity(loginModel);

            var json = JsonConvert.SerializeObject(userEntity, Formatting.Indented);

            Console.WriteLine("Please paste the following json into the application config file:");
            Console.WriteLine();

            Console.WriteLine(json);
        }
    }
}
