using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.ProductCategory
{
    public interface IProductCategoryService : ICrudService<int, ProductCategoryReadDto, ProductCategoryCreateUpdateDto, ProductCategoryCreateUpdateDto>
    {
    }
}
