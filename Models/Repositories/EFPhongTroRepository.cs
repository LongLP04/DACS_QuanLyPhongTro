using Microsoft.EntityFrameworkCore;

namespace DACS_QuanLyPhongTro.Models.Repositories
{
    public class EFPhongTroRepository : IPhongTroRepository
    {
        private readonly ApplicationDbContext _context;

        public EFPhongTroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhongTro>> GetAllAsync()
        {
            return await _context.PhongTros
                                 .Include(p => p.ToaNha)
                                    .ThenInclude(t => t.ChuTro)
                                 .Include(p => p.KhachThue)
                                 
                                 .ToListAsync();
        }

        public async Task<PhongTro?> GetByIdAsync(int id)
        {
            return await _context.PhongTros
                                 .Include(p => p.ToaNha)
                                    .ThenInclude(t => t.ChuTro)
                                 .Include(p => p.KhachThue)
                                 .FirstOrDefaultAsync(p => p.MaPhong == id);
        }

        public async Task AddAsync(PhongTro phongTro)
        {
            await _context.PhongTros.AddAsync(phongTro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhongTro phongTro)
        {
            _context.PhongTros.Update(phongTro);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var phongTro = await _context.PhongTros.FindAsync(id);
            if (phongTro != null)
            {
                _context.PhongTros.Remove(phongTro);
                await _context.SaveChangesAsync();
            }
        }
    }
}
