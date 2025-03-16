import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { UserModel } from "./types";

export type State = {
    user: UserModel | null | undefined;
    image: string | null;
    loaded: boolean;
    error: string | null;
}

const initialState: State = {
    user: undefined,
    image: null,
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
        setImage: (state, action: PayloadAction<ActionModel<State['image']>>) => {
            state.image = action.payload.data;
            state.error = action.payload.error;
            state.loaded = true;
        },
    },
    extraReducers(builder) {
        builder.addDefaultCase((state) => {
            state.loaded = true;
            state.error = null;
        });
    },
});

export const {
    setUser,
    setImage
} = store.actions;

export default store.reducer;