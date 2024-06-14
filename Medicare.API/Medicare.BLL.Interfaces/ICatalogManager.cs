using Medicare.DAL.Models;

namespace Medicare.BLL.Interfaces;

public interface ICatalogManager
{
    List<Specialization> GetSpecializations();
}
