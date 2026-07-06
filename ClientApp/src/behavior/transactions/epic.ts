import { combineEpics, Epic, ofType } from "redux-observable";
import { delay, map } from "rxjs";
import { createAction } from "@reduxjs/toolkit";
import {
    PaymentAttempt,
    setTransactions,
    updateTransactionStatus,
} from "./reducer";

// load list
export const loadTransactions = createAction("transactions/load");

// refund request
export const refundTransaction = createAction<string>("transactions/refund");

// ---------------- MOCK DATA ----------------

const mockTx: PaymentAttempt[] = [
    {
        id: "1",
        orderId: "o1",
        userId: "u1",
        amount: 29.99,
        status: "Succeeded",
        provider: "Stripe",
        isPrimary: true,
        createdAt: new Date().toISOString(),
    },
    {
        id: "2",
        orderId: "o2",
        userId: "u1",
        amount: 9.99,
        status: "Pending",
        provider: "Stripe",
        isPrimary: false,
        createdAt: new Date().toISOString(),
    },
];

// load
export const loadTransactionsEpic: Epic = (action$) =>
    action$.pipe(
        ofType(loadTransactions.type),
        map(() => setTransactions(mockTx))
    );

// refund
export const refundEpic: Epic = (action$) =>
    action$.pipe(
        ofType(refundTransaction.type),
        delay(1000),
        map((action: any) =>
            updateTransactionStatus({
                id: action.payload,
                status: "Refunded",
            })
        )
    );

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(loadTransactionsEpic, refundEpic)
        (action$, store$, dependencies);

export default rootEpic;