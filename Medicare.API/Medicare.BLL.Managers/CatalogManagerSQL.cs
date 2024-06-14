using Medicare.BLL.Interfaces;
using Medicare.DAL.Models;
using Medicare.DAL.Storage;

namespace Medicare.BLL.Managers;

public class CatalogManagerSQL : ICatalogManager
{
    private MedicareContext _context;

    public CatalogManagerSQL(MedicareContext context)
    {
        _context = context;
    }

    public List<Specialization> GetSpecializations()
    {
        var specializations = _context.Specializations.ToList();

        return specializations;
    }
}
