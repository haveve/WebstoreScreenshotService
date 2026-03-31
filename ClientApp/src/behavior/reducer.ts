import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Screenshot, UserModel } from "./types";

type ScreenshotState = {
    screenshot: Screenshot | null,
    lastChangedDate: string
}

export type State = {
    user: UserModel | null | undefined;
    screenshot: ScreenshotState | null;
    loaded: boolean;
    error: string | null;
}

const initialState: State = {
    user: undefined,
    screenshot: null,
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
        setScreenshot: (state, action: PayloadAction<ActionModel<ScreenshotState['screenshot']>>) => {
            state.screenshot = {screenshot: action.payload.data, lastChangedDate: new Date().toISOString()};
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
    setUser,
    setScreenshot
} = store.actions;

export default store.reducer;