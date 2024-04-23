﻿namespace Kentico.Xperience.Shopify.Models
{
    public class ListResultWrapper<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public PagingFilterParams? PrevPageFilter { get; set; }
        public PagingFilterParams? NextPageFilter { get; set; }
    }
}

