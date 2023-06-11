using System.Collections.Generic;

namespace Topic.API.Models
{
    public class TopicCategory
    {
        public string Id { get; }
        public int DisplayOrder { get; }
        public string Name { get; }
        public string PictureFilePath { get; set; }
        public ICollection<TopicCategoryTags> CategoryTags { get; } = new List<TopicCategoryTags>();

        public TopicCategory(string id, int displayOrder, string name, string pictureFilePath)
        {
            Id = id;
            DisplayOrder = displayOrder;
            Name = name;
            PictureFilePath = pictureFilePath;
        }
    }
}
