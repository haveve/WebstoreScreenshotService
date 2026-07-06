import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export type OrderStatus = "PendingPayment" | "Paid" | "Cancelled";

export type OrderLine = {
  id: string;
  orderId: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  productName: string;
};

export type Order = {
  id: string;
  userId: string;
  totalAmount: number;
  currency: string;
  status: OrderStatus;
  createdAt: string;
  lines: OrderLine[];
};

type State = {
  items: Order[];
  selected?: Order;
};

const initialState: State = {
  items: [],
};

export const orderSlice = createSlice({
  name: "orders",
  initialState,
  reducers: {
    setOrders: (state, action: PayloadAction<Order[]>) => {
      state.items = action.payload;
    },

    setOrder: (state, action: PayloadAction<Order>) => {
      state.selected = action.payload;
    },
  },
});

export const { setOrders, setOrder } = orderSlice.actions;
export default orderSlice.reducer;