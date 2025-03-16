import { configureStore, Tuple } from "@reduxjs/toolkit";
import { createEpicMiddleware } from "redux-observable";
import rootEpic from "./epic";
import reducer, { State } from "./reducer";
import { useSelector } from "react-redux";

const epicMiddleware = createEpicMiddleware();

const store = configureStore({
    reducer,
    middleware: () => new Tuple(epicMiddleware)
});

epicMiddleware.run(rootEpic);

export const useAppSelector = useSelector.withTypes<State>()

export default store;