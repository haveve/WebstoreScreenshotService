import { configureStore, Tuple, combineReducers } from "@reduxjs/toolkit";
import { combineEpics, createEpicMiddleware } from "redux-observable";
import rootEpic from "./epic";
import reducer from "./reducer";
import basketReducer from './basket/reducer'
import basketEpic from './basket/epic';
import transactionsReducer from './transactions/reducer'
import transactionsEpic from './transactions/epic'
import orderReducer from './order/reducer'
import orderEpic from './order/epic'
import { useSelector } from "react-redux";

const combinedEpics = combineEpics(rootEpic, basketEpic, transactionsEpic, orderEpic);

const combinedReducers = combineReducers({
    basic: reducer,
    basket: basketReducer,
    transactions: transactionsReducer,
    orders: orderReducer
});

const epicMiddleware = createEpicMiddleware();
const store = configureStore({
    reducer: combinedReducers,
    middleware: () => new Tuple(epicMiddleware)
});

epicMiddleware.run(combinedEpics);

export type RootState = ReturnType<typeof store.getState>;
export const useAppSelector = useSelector.withTypes<RootState>()

export default store;