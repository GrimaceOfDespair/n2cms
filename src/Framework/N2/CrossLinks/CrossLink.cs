using N2.Details;

namespace N2.CrossLinks
{
    public class CrossLink<T> : ContentItem where T : ContentItem
    {
        [EditableLink]
        public T ContentItem { get; set; }
    }
}
