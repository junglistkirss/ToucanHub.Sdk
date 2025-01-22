//using Toucan.Sdk.Contracts.Query.Page;

//namespace Toucan.Sdk.Contracts.Extensions;

//public static class Paging
//{
//    //public static SortOption<T> CreateSortOption<T>(string? sort = null, bool desc = false)
//    //         where T : struct
//    //{
//    //    T? sorting = null;
//    //    try
//    //    {

//    //        if (typeof(T).IsEnum)
//    //            _ = sort.TryConvertOrNull(out sorting);
//    //        else if (typeof(T).IsAssignableTo(typeof(InlineParsable<T>)))
//    //        {
//    //            sorting = (typeof(T).GetMethod(nameof(InlineParsable<T>.Parse), BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [sort])) as T?;
//    //        }
//    //        else
//    //            sorting = Convert.ChangeType(sort, typeof(T)) as T?;

//    //    }
//    //    catch (Exception) { }
//    //    return new SortOption<T>
//    //    {
//    //        Field = sorting,
//    //        Direction = desc ? SortDirection.DESC : SortDirection.ASC,
//    //    };
//    //}
//    public static SortOption<T>[] SimpleSortOptions<T>(T? sortBy = default, SortDirection? sortDirection = SortDirection.ASC)
//            where T : notnull
//    {
//        return [new SortOption<T>
//        {
//            Field = sortBy,
//            Direction = sortDirection ?? SortDirection.ASC,
//        }];
//    }
//    public static SortOption<T>[] SimpleSortOptions<T>(T? sort = default, bool? desc = false)
//             where T : notnull
//    {
//        //_ = sort.TryConvertOrNull(out T? sorting);
//        return [new SortOption<T>
//        {
//            Field = sort,
//            Direction = desc ?? false ? SortDirection.DESC : SortDirection.ASC,
//        }];
//    }
//    //public static SortOption<TSortEnum>[] CreateSortOptions<TSortEnum>(string? inline)
//    //         where TSortEnum : struct
//    //{
//    //    if (string.IsNullOrEmpty(inline))
//    //        return [SortOption<TSortEnum>.Empty];

//    //    List<SortOption<TSortEnum>> sorts = [];
//    //    string[] split = inline.Split(',');
//    //    foreach (string part in split)
//    //    {
//    //        SortOption<TSortEnum>? sort;
//    //        if (part.Contains('.'))
//    //        {
//    //            int i = part.IndexOf('.');
//    //            string direction = part[..i];
//    //            if (direction.Contains('d', StringComparison.OrdinalIgnoreCase))
//    //                sort = CreateSortOption<TSortEnum>(part[i..], true);
//    //            else
//    //                sort = CreateSortOption<TSortEnum>(part[i..], false);
//    //        }
//    //        else
//    //        {
//    //            sort = CreateSortOption<TSortEnum>(part, false);
//    //        }

//    //        if (sort.HasValue)
//    //            sorts.Add(sort.Value);
//    //    }
//    //    return [.. sorts];
//    //}


//    public static Pagination CreatePagination(int? offset = 0, int? limit = null, int defaultLimit = 100, int upLimit = 10000)
//    {
//        return new Pagination()
//        {
//            Limit = Math.Max(1, Math.Min(limit ?? defaultLimit, upLimit)),
//            Offset = Math.Max(0, offset ?? 0),
//        };
//    }

//}
