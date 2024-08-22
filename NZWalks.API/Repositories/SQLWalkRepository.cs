using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using System.Linq;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        public NZWalksDbContext NZWalksDbContext { get; }
        public SQLWalkRepository(NZWalksDbContext nZWalksDbContext)
        {
            NZWalksDbContext = nZWalksDbContext;
        }

        

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await NZWalksDbContext.Walks.AddAsync(walk);
            await NZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 5)
        {
            var walks = NZWalksDbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            // filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false )
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
                if (filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Description.Contains(filterQuery));
                }

            }

            // sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false) 
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            // pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
           
            
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            var walk = await NZWalksDbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(r => r.Id == id);
            if(walk == null) return null;
            return walk;
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walkDomainModel)
        {
            var walk = await GetByIdAsync(id);
            if (walk == null)
            {
                return null;
            }
            walk.Name = walkDomainModel.Name;
            walk.Description = walkDomainModel.Description;
            walk.RegionId = walkDomainModel.RegionId;
            walk.WalkImageUrl = walkDomainModel.WalkImageUrl;
            walk.LengthInKm = walkDomainModel.LengthInKm;
            walk.DifficultyId = walkDomainModel.DifficultyId;


            await NZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var walk = await GetByIdAsync(id);
            if (walk == null)
            {
                return null;
            }
            NZWalksDbContext.Walks.Remove(walk);
            await NZWalksDbContext.SaveChangesAsync();
            return walk;
        }
    }
}
