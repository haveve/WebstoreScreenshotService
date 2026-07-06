import { combineEpics, Epic, ofType } from "redux-observable";
import { map, exhaustMap, delay, of } from "rxjs";
import { createAction } from "@reduxjs/toolkit";
import { clearBasket } from "./reducer";
import { Duration, SubscriptionType } from "../types";

// -------------------- ACTIONS --------------------

export const createSubscriptionCheckout = createAction<{
    plan: SubscriptionType;
    duration: Duration;
    price: number;
}>("checkout/subscription");

export const createBasketCheckout = createAction("checkout/basket");

export const checkoutSuccess = createAction("checkout/success");

// -------------------- EPICS --------------------

export const subscriptionEpic: Epic = (action$) =>
    action$.pipe(
        ofType(createSubscriptionCheckout.type),
        exhaustMap((action: any) => {
            // MOCK STRIPE FLOW
            return of(action.payload).pipe(
                delay(1200),
                map(() => checkoutSuccess())
            );
        })
    );

export const basketCheckoutEpic: Epic = (action$) =>
    action$.pipe(
        ofType(createBasketCheckout.type),
        exhaustMap(() => {
            return of([1, 2]).pipe(
                delay(1200),
                map(() => {
                    return clearBasket();
                })
            );
        })
    );

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(subscriptionEpic, basketCheckoutEpic)
        (action$, store$, dependencies);

export default rootEpic;