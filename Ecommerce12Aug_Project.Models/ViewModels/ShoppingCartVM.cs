using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce12Aug_Project.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
