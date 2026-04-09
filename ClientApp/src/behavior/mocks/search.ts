import { Paging, SearchScope } from "../types";
import { mockScreenshots } from "./screenshots";

type ScreenshotWithCategory = typeof mockScreenshots[number];

export const mockSearchScreenshots = async (
    paging: Paging
): Promise<{ items: ScreenshotWithCategory[]; total: number }> => {

    // simulate network delay
    await new Promise(res => setTimeout(res, 400));

    let data = [...mockScreenshots];

    // QUERY FILTER
    if (paging.query?.trim()) {
        const q = paging.query.toLowerCase();

        data = data.filter(s => {
            const title = s.title?.toLowerCase() ?? "";
            const description = s.description?.toLowerCase() ?? "";

            switch (paging.searchScope) {
                case SearchScope.Title:
                    return title.includes(q);

                case SearchScope.All:
                    return (
                        title.includes(q) ||
                        description.includes(q)
                    );
                default:
                    return true;
            }
        });
    }

    // CATEGORY FILTER
    if (paging.categoryIds?.length) {
        data = data.filter(s =>
            s.categories.some(({ id }) =>
                paging.categoryIds!.includes(id)
            )
        );
    }

    // SORT (newest first — realistic default)
    data.sort(
        (a, b) =>
            new Date(b.createdAt).getTime() -
            new Date(a.createdAt).getTime()
    );

    const total = data.length;

    // PAGING
    const start = (paging.page - 1) * paging.pageSize;
    const items = data.slice(start, start + paging.pageSize);

    return { items, total };
};