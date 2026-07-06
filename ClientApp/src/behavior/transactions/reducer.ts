import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export type PaymentAttempt = {
  id: string;
  orderId: string;
  userId: string;
  amount: number;
  status: "Pending" | "Succeeded" | "Failed" | "Refunded";
  provider: string;
  providerPaymentId?: string;
  isPrimary: boolean;
  createdAt: string;
};

type State = {
  items: PaymentAttempt[];
  loaded: boolean;
};

const initialState: State = {
  items: [],
  loaded: false,
};

export const transactionsSlice = createSlice({
  name: "transactions",
  initialState,
  reducers: {
    setTransactions: (state, action: PayloadAction<PaymentAttempt[]>) => {
      state.items = action.payload;
      state.loaded = true;
    },

    updateTransactionStatus: (
      state,
      action: PayloadAction<{ id: string; status: PaymentAttempt["status"] }>
    ) => {
      const tx = state.items.find(t => t.id === action.payload.id);
      if (tx) tx.status = action.payload.status;
    },

    addTransaction: (state, action: PayloadAction<PaymentAttempt>) => {
      state.items.unshift(action.payload);
    },
  },
});

export const {
  setTransactions,
  updateTransactionStatus,
  addTransaction,
} = transactionsSlice.actions;

export default transactionsSlice.reducer;