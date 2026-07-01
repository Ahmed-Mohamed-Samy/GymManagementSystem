using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagementDAL.DataSeed
{
    public static class GymDbContextDataSeeding
    {
        public static async Task<bool> SeedDataAsync(GymDbContext dbContext)
        {
            try
            {
                var HasPlans = await dbContext.Plans.AnyAsync();
                var HasCategories = await dbContext.Categories.AnyAsync();


                if (HasPlans && HasCategories) return false;

                if (!HasPlans)
                {
                    var plans = await LoadDataFromJsonFileAsync<Plan>("Plans.json");

                    if (plans.Any())
                        await dbContext.Plans.AddRangeAsync(plans);

                }

                if (!HasCategories)
                {
                    var Categories = await LoadDataFromJsonFileAsync<Category>("Categories.json");

                    if (Categories.Any())
                        await dbContext.Categories.AddRangeAsync(Categories);
                }

                return await dbContext.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Seeding Failed {ex}");
                return false;
            }
        }


        private static async Task<List<T>> LoadDataFromJsonFileAsync<T>(string fileName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);

            if (!File.Exists(FilePath)) throw new FileNotFoundException();

            string Data = await File.ReadAllTextAsync(FilePath);

            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };


            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? []; 



        }

    }
}
