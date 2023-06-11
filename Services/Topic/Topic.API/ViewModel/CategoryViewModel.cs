using System.Collections.Generic;

namespace Topic.API.ViewModel
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PictureFilePath { get; set; }
        public IEnumerable<TagViewModel> Tags { get; set; }
    }
}
