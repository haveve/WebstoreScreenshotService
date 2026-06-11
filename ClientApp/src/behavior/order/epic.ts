import { combineEpics, Epic, ofType } from "redux-observable";
import { map } from "rxjs";
import { createAction } from "@reduxjs/toolkit";
import { setOrders, setOrder, Order } from "./reducer";

// load list
export const loadOrders = createAction("orders/load");

// load single
export const loadOrder = createAction<string>("orders/loadOne");

// ---------------- MOCK DATA ----------------

export const mockOrders: Order[] = [
  {
    id: "ORD-1001",
    userId: "user-1",
    totalAmount: 3.98,
    currency: "USD",
    status: "Paid",
    createdAt: new Date(Date.now() - 86400000 * 2).toISOString(),
    lines: [
      {
        id: "line-1",
        orderId: "ORD-1001",
        productId: "points-5000",
        quantity: 1,
        unitPrice: 3.98,
        productName: "5,000 балів",
      },
    ],
  },

  {
    id: "ORD-1002",
    userId: "user-1",
    totalAmount: 19.90,
    currency: "USD",
    status: "Paid",
    createdAt: new Date(Date.now() - 86400000).toISOString(),
    lines: [
      {
        id: "line-2",
        orderId: "ORD-1002",
        productId: "points-10000",
        quantity: 1,
        unitPrice: 19.90,
        productName: "10,000 балів",
      },
    ],
  },

  {
    id: "ORD-1003",
    userId: "user-1",
    totalAmount: 299.99,
    currency: "USD",
    status: "PendingPayment",
    createdAt: new Date().toISOString(),
    lines: [
      {
        id: "line-3",
        orderId: "ORD-1003",
        productId: "advanced-yearly",
        quantity: 1,
        unitPrice: 299.99,
        productName: "Підписка Advanced (Річна)",
      },
    ],
  },
];

// list
export const loadOrdersEpic: Epic = (action$) =>
    action$.pipe(
        ofType(loadOrders.type),
        map(() => setOrders(mockOrders))
    );

// single
export const loadOrderEpic: Epic = (action$) =>
    action$.pipe(
        ofType(loadOrder.type),
        map((action: any) =>
            setOrder(mockOrders.find(o => o.id === action.payload)!)
        )
    );

const rootEpic: Epic = (action$, store$, dependencies) =>
    combineEpics<any>(loadOrderEpic, loadOrdersEpic)
        (action$, store$, dependencies);

export default rootEpic;