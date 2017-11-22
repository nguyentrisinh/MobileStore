using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class Model
    {
        public int ModelID { get; set; }
        public string Name { get; set; }
        public ModelType Type { get; set; }
        
        public enum ModelType
        {
            Device = 1,
            Accessory = 2,
        }
    }
}
