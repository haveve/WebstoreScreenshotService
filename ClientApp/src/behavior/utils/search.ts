import { Paging, SearchScope } from "../types";

export const DEFAULT_PAGE_SIZE = 20;

export const MAX_PAGE_COUNT = 10_000;
export const MAX_PAGE_SIZE = 200;

export const parseSearchScope = (value: string | null): SearchScope => {
    const raw = Number(value);
    const isValid = SearchScope.All === raw || SearchScope.Title === raw;
    return Number.isNaN(raw) || !isValid ? SearchScope.All : raw as SearchScope;
};

export const formatQueryPaging = (): Paging => {
    const params = new URLSearchParams(window.location.search);

    const page = Math.min(Math.max(1, Number(params.get("page")) || 1), MAX_PAGE_COUNT);
    const pageSize = Math.min(MAX_PAGE_SIZE, Math.max(1, Number(params.get("pageSize")) || DEFAULT_PAGE_SIZE));

    const query = params.get("query") ?? "";

    const searchScope = parseSearchScope(params.get("searchScope"));

    const categoryIds = params.get("categoryIds")
        ? params.get("categoryIds")!.split(",").filter(Boolean)
        : [];

    return {
        page,
        pageSize,
        query,
        searchScope,
        categoryIds
    };
};

export const setPagingQuery = (paging: Paging) => {
    const params = new URLSearchParams();

    params.set("page", String(paging.page));
    params.set("pageSize", String(paging.pageSize));

    if (paging.query)
        params.set("query", paging.query);

    params.set("searchScope", String(paging.searchScope));

    if (paging.categoryIds?.length)
        params.set("categoryIds", paging.categoryIds.join(","));

    const newUrl = `${window.location.pathname}?${params.toString()}`;

    window.history.replaceState({}, "", newUrl);
};