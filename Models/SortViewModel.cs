namespace WEB_Shop_Ajax.Models
{
    public class SortViewModel
    {
        public SortState BrandSort { get; private set; } // значение для сортировки по имени
        public SortState PriceSort { get; private set; }    // значение для сортировки по возрасту
        public SortState ModelSort { get; private set; }   // значение для сортировки по компании
        public SortState Current { get; private set; }     // текущее значение сортировки

        public SortViewModel(SortState sortOrder)
        {
            BrandSort = sortOrder == SortState.BrandAsc ? SortState.BrandDesc : SortState.BrandAsc;
            PriceSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            ModelSort = sortOrder == SortState.ModelAsc ? SortState.ModelDesc : SortState.ModelAsc;
            Current = sortOrder;
        }
    }
}
