using System;
using System.Linq;
using AMRVI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class DbCheck
{
    public static void Run(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        Console.WriteLine("--- CHECKING MACHINES ---");
        
        var plants = new[] { "RVI", "BTR", "HOSE", "MOLDED", "MIXING" };
        
        foreach (var plant in plants)
        {
            int count = 0;
            switch (plant)
            {
                case "RVI": count = context.Machines.Count(); break;
                case "BTR": count = context.Machines_BTR.Count(); break;
                case "HOSE": count = context.Machines_HOSE.Count(); break;
                case "MOLDED": count = context.Machines_MOLDED.Count(); break;
                case "MIXING": count = context.Machines_MIXING.Count(); break;
            }
            Console.WriteLine($"Plant {plant}: {count} machines");
            
            if (count > 0)
            {
                Console.WriteLine("Samples:");
                if (plant == "RVI") {
                   foreach(var m in context.Machines.Take(3)) Console.WriteLine($" - ID: {m.Id}, Name: {m.Name}");
                } else if (plant == "HOSE") {
                   foreach(var m in context.Machines_HOSE.Take(3)) Console.WriteLine($" - ID: {m.Id}, Name: {m.Name}");
                }
            }
        }
    }
}
