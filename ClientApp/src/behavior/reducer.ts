import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Category, PagedResult, Screenshot, SubscriptionType, UserModel } from "./types";

type ScreenshotState = {
    screenshot: Screenshot | null,
    lastChangedDate: string
}

type ScreenshotListState = {
    items: Screenshot[];
    total: number;
    page: number;
    pageSize: number;
};

type CategoryState = {
    items: Category[];
};

export type State = {
    user: UserModel | null | undefined;
    screenshot: ScreenshotState | null;
    screenshots: ScreenshotListState | null;
    categories: CategoryState | null;
    loaded: boolean;
    error: string | null;
}

const initialState: State = {
    user: undefined,
    screenshot: null,
    screenshots: null,
    categories: null,
    loaded: false,
    error: null
}

type ActionModel<T> = {
    error: string | null;
    data: T;
}

export const store = createSlice({
    name: "store",
    initialState,
    reducers: {
        setUser: (state, action: PayloadAction<ActionModel<State['user']>>) => {
            state.user = action.payload.data;
            state.error = action.payload.error;
            state.loaded = true;
        },
        cancelSubscription: (state, action: PayloadAction<ActionModel<undefined>>) => {
            if (state.user?.subscriptionPlan)
                state.user.subscriptionPlan.type = SubscriptionType.Regular;

            state.error = action.payload.error;
            state.loaded = true;
        },
        setScreenshot: (state, action: PayloadAction<ActionModel<ScreenshotState['screenshot']>>) => {
            state.screenshot = { screenshot: action.payload.data, lastChangedDate: new Date().toISOString() };
            state.error = action.payload.error;
            state.loaded = true;
        },
        setScreenshots: (
            state,
            action: PayloadAction<ActionModel<PagedResult<Screenshot> & { page: number, pageSize: number } | null>>
        ) => {
            state.loaded = true;

            if (action.payload.error)
                state.error = action.payload.error;

            state.screenshots = action.payload.data ? {
                items: action.payload.data.items,
                total: action.payload.data.total,
                page: action.payload.data.page,
                pageSize: action.payload.data.pageSize
            } : null;

        },

        setCategories: (
            state,
            action: PayloadAction<ActionModel<Category[] | null>>
        ) => {
            state.categories = action.payload.data ? {
                items: action.payload.data
            } : null;

            state.error = action.payload.error;
            state.loaded = true;
        },
    },
    extraReducers(builder) {
        builder.addDefaultCase((state) => {
            state.loaded = false;
            state.error = null;
        });
    },
});

export const {
    cancelSubscription,
    setUser,
    setScreenshot,
    setScreenshots,
    setCategories
} = store.actions;

export default store.reducer;