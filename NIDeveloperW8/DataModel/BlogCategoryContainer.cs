using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NIDeveloperW8.Data
{
    [DataContract]
    public class BlogCategoryContainer
    {
        [DataMember]
        public int length;

        [DataMember]
        public CategoryModel[] categories;
    }
}